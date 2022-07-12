using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public abstract class BasePlayer : MonoBehaviourPunCallbacks, IGemContainer, IGamePanelObserver, IOnEventCallback
{
    protected const float SPEED_MULTIPLIER = 0.01f;

    // Имя

    [SerializeField]
    protected Text NameT;
    protected string Name = "";
    public void SetName(string newName) 
    { 
        Name = newName;
        if (NameT != null)
            NameT.text = Name; 
    }
    public string GetName() { return Name; }

    // Скин

    protected Color Skin = Color.white;
    public void SetSkin(Color newSkin) 
    { 
        Skin = newSkin;
        if (!IsMine())
        {
            if (visibility)
                myrenderer.color = Skin;
            else
            {
                Color c = Skin;
                c.a = 0f;
                myrenderer.color = c;
            }
        }
        else
        {
            myrenderer.color = Skin;
        }
    }
    public Color GetSkin() { return Skin; }

    // Гемы

    protected int gems { get; set; }
    [SerializeField]
    protected GamePanel gempanel;
    [SerializeField]
    private Text gemsT;

    // энергия

    protected int energy = 20;
    [SerializeField]
    protected GameObject EnergyBomb;
    private const float ENERGY_DELTA_TIME = 0.4f; // за сколько секунд код добавляет 1 ед энергии
    public const int MAX_ENERGY = 100;
    public void AddEnergy(int newenergy)
    {
        energy = Mathf.Clamp(energy + newenergy, 0, MAX_ENERGY);
    }
    public void SetEnergy(int newenergy)
    {
        energy = Mathf.Clamp(newenergy, 0, MAX_ENERGY);
    }
    public int GetEnergy() { return energy; }

    // Прочее

    public int ID { get; protected set; }

    public float speed;

    [SerializeField]
    protected SpriteRenderer myrenderer;

    [SerializeField]
    protected PhotonView photonview;

    protected Chancel MyChancel;

    public Vector2 SpawnPoint = Vector2.zero;

    public bool visibility { get; private set; }

    public bool IsPlaying;

    [SerializeField]
    private float paralizetime;
    

    protected virtual void Start()
    {
        visibility = true;
        gempanel.AddObserver(this);
        gempanel.Close();
    }

    protected virtual void Update()
    {
        if(!wasaddingenergy && IsPlaying)
        {
            wasaddingenergy = true;
            StartCoroutine(AddingEnergy());
        }
    }

    protected bool IsMine()
    {
        return photonview != null && photonview.IsMine
            || MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline && !isBot();
    }

    public virtual void BecomeVisible()
    {
        visibility = true;
        if (gems > 0)
            gempanel.Open();
        else
            gempanel.Close();
        myrenderer.color = Skin;
        if (NameT != null)
            NameT.gameObject.SetActive(true);
    }

    public virtual void BecomeInvisible()
    {
        if (!IsMine())
        {
            visibility = false;
            gempanel.Close();
            Color c = myrenderer.color;
            c.a = 0f;
            myrenderer.color = c;
            NameT.gameObject.SetActive(false);
        }
    }

    public bool GetGem()
    {
        if (gems > 0)
        {
            gems -= 1;
            if (gems == 0)
                gempanel.Close();
            else
                gempanel.Open();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddGem()
    {
        if (IsPlaying)
        {
            gems++;
            if (visibility)
                gempanel.Open();
        }
    }

    protected void AddGems(int newgems)
    {
        if (IsPlaying)
        {
            gems = Mathf.Clamp(gems + newgems, 0, 5);
            if (visibility)
                gempanel.Open();
        }
    }

    public int GemAmount()
    {
        return gems;
    }

    public bool CanBeTaken()
    {
        return false;
    }

    public bool CanBeAdded()
    {
        return false;
    }


    public void OnOpened()
    {
        if (gems > 1)
            gemsT.text = gems.ToString();
        else
            gemsT.text = "";
    }

    public void OnClosed() {}


    private bool wasaddingenergy; 
    private IEnumerator AddingEnergy()
    {
        yield return new WaitForSeconds(ENERGY_DELTA_TIME);
        AddEnergy(1);
        wasaddingenergy = false;
    }
    protected void ShootEnergy()
    {
        if(energy >= 50)
        {
            GameObject bomb;
            if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
            {
                bomb = PhotonNetwork.Instantiate(EnergyBomb.name, transform.position, Quaternion.identity);
            }
            else
                bomb = Instantiate(EnergyBomb, transform.position, Quaternion.identity);
            bomb.GetComponent<EnergyBomb>().myplayer = this;
            energy -= 50;
        }
    }

    public void HitbyBomb()
    {
        StartCoroutine(ParalyzingByBomb());
    }

    private IEnumerator ParalyzingByBomb()
    {
        BecomeVisible();
        IsPlaying = false;
        OnBeingHitbyBomb();
        yield return new WaitForSeconds(paralizetime * 0.4f);
        if (photonview != null && photonview.IsMine || MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline)
            transform.position = SpawnPoint;
        yield return new WaitForSeconds(paralizetime * 0.6f);
        if(!IsMine())
        {
            BecomeInvisible();
        }
        IsPlaying = true;
    }

    protected abstract void OnBeingHitbyBomb();

    protected abstract bool isBot();

    public void OnEvent(EventData photonEvent)
    {
        switch(photonEvent.Code)
        {
            case 3:
                if((int)photonEvent.CustomData == ID)
                {
                    if (photonview.IsMine)
                        GetGem();
                    HitbyBomb();
                }
                break;
        }
    }

    protected abstract void onEvent(EventData photonEvent);
}
