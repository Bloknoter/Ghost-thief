using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Chancel : MonoBehaviour, IGemContainer
{

    [SerializeField]
    private SyncMapObject syncnetwork;

    [SerializeField]
    private GameObject[] gemsIndicators = new GameObject[GameController.GEMS_TO_WIN];

    [SerializeField]
    protected int gems;

    [SerializeField]
    private AudioSource GettingGemAudio;

    private List<IChancelObserver> observers = new List<IChancelObserver>();


    private bool raiseeventminus1 = true;  /* Нужно ли посылать сообщение по сети своему двойнику о 
                                 взятии кристалла или я сам получил это сообщение и посылать не нужно? */

    private bool raiseeventplus1 = true;  /* Нужно ли посылать сообщение по сети своему двойнику о 
                                 прибавлении 1 кристалла или я сам получил это сообщение и посылать не нужно? */


    public bool GetGem()
    {
        if (gems > 0)
        {
            if (raiseeventminus1)
            {
                if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
                {
                    object data = -1;
                    syncnetwork.SendData(data);
                }

            }
            raiseeventminus1 = true;
            gemsIndicators[gems - 1].SetActive(false);
            gems -= 1;
            if (gems > 0)
                gemsIndicators[gems - 1].SetActive(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddGem()
    {
        if (raiseeventplus1)
        {
            if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
            {
                object data = 1;
                syncnetwork.SendData(data);
            }
            
        }
        raiseeventplus1 = true;
        if (gems > 0)
            gemsIndicators[gems - 1].SetActive(false);
        gems++;
        GettingGemAudio.Play();
        gemsIndicators[gems - 1].SetActive(true);
        if(gems == GameController.GEMS_TO_WIN)
        {
            OnWin();
        }
    }

    public int GemAmount()
    {
        return gems;
    }

    public bool CanBeTaken()
    {
        return true;
    }

    public bool CanBeAdded()
    {
        return true;
    }

    void Start()
    {
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
        foreach(var g in gemsIndicators)
        {
            g.SetActive(false);
        }
    }

    private void OnWin()
    {
        foreach(var ob in observers)
        {
            ob.OnCollectingAllGems(this);
        }
    }

    public void AddObserver(IChancelObserver newobserver)
    {
        observers.Add(newobserver);
    }

    public void RemoveObserver(IChancelObserver thisobserver)
    {
        observers.Remove(thisobserver);
    }

    private void GetData(object getdata)
    {
        switch ((int)getdata)
        {
            case 1:
                raiseeventplus1 = false;
                AddGem();
                break;
            case -1:
                raiseeventminus1 = false;
                GetGem();
                break;
        }
    }

}
