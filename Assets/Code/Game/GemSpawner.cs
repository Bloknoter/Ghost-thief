using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GemSpawner : MonoBehaviour, IGemContainer
{
    [SerializeField]
    private SyncMapObject syncnetwork;

    [SerializeField]
    private GameObject[] gemsIndicators;

    [SerializeField]
    private int gems = 3;

    [SerializeField]
    private Light BlueLight;
    private float startlightintesity;

    [SerializeField]
    private ParticleSystem[] fireparticles;

    [SerializeField]
    private SpriteRenderer Runes;

    private int getgemcalls = 0;

    private bool raiseevent = true;  /* Нужно ли посылать сообщение по сети своему двойнику о 
                                 взятии кристалла или я сам получил это сообщение и посылать не нужно? */

    [SerializeField]
    private AudioSource fire;
    public void AddGem()
    {
    }

    public bool CanBeAdded()
    {
        return false;
    }

    public bool CanBeTaken()
    {
        return true;
    }

    public int GemAmount()
    {
        return gems;
    }

    public bool GetGem()
    {
        if (gems > 0)
        {
            gemsIndicators[gems - 1].SetActive(false);
            getgemcalls += 1;
            gems -= 1;
            fire.Play();
            if (gems > 0)
            {
                gemsIndicators[gems - 1].SetActive(true);
                if (getgemcalls == 1)
                    StartCoroutine(FlashToGetGem());
            }
            else
            {
                if (getgemcalls == 1)
                    StartCoroutine(FlashToGetLastGem());
            }
            if (raiseevent)
            {
                if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
                {
                    syncnetwork.SendData(null);
                }
            }
            raiseevent = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator FlashToGetGem()
    {
        
        for (int i = 0; i < 50; i++)
        {
            BlueLight.intensity += startlightintesity / 100f;
            yield return new WaitForSeconds(0f);
        }
        for (int i = 0; i < 50; i++)
        {
            BlueLight.intensity -= startlightintesity / 100f;
            yield return new WaitForSeconds(0f);
        }
        getgemcalls -= 1;
        CheckGetGemCalls();
    }

    private void CheckGetGemCalls()
    {
        if(getgemcalls > 0)
        {
            if(gems > 0)
            {
                StartCoroutine(FlashToGetGem());
            }
            else
            {
                StartCoroutine(FlashToGetLastGem());
            }
        }
    }

    private IEnumerator FlashToGetLastGem()
    {
        for (int i = 0; i < 50; i++)
        {
            BlueLight.intensity += startlightintesity / 100f;
            yield return new WaitForSeconds(0f);
        }
        const float colordelta = 0.015f;
        foreach(var p in fireparticles)
        {
            ParticleSystem.MainModule module = p.main;
            module.loop = false;
        }
        for (int i = 0; i < 150; i++)
        {
            BlueLight.intensity -= startlightintesity / 100f;
            Color c = Runes.color;
            c = new Color(c.r - colordelta, c.g - colordelta, c.b - colordelta);
            Runes.color = c;
            yield return new WaitForSeconds(0f);
        }
        getgemcalls = 0;
    }

    void Start()
    {
        startlightintesity = BlueLight.intensity;
        if (gems == 0 || gems < 0)
        {
            gems = 0;
            SetZeroIndicator();
        }
        else
        {
            SetZeroIndicator();
            gemsIndicators[gems - 1].SetActive(true);
        }
        syncnetwork.SetCallBack(GetData);
    }

    void Update()
    {
        
    }

    protected void SetZeroIndicator()
    {
        foreach (var g in gemsIndicators)
        {
            g.SetActive(false);
        }
    }

    private void GetData(object getdata)
    {
        raiseevent = false;
        GetGem();
    }
}
