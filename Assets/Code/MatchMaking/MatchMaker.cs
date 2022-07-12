using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

using CuriousAttributes;

public class MatchMaker : MonoBehaviourPunCallbacks
{
    [Tab("Data objects")]
    [SerializeField]
    [Label("Game data")]
    public GameData gameData;

    [Tab("Data objects")]
    [SerializeField]
    [Label("Skins data")]
    public SkinsData skinsData;

    [SerializeField]
    [Label("Maps data")]
    [Tab("Data objects")]
    public MapsData mapsData;


    [SerializeField]
    [Label("Match board log")]
    [Tab("Interacting scripts")]
    public ChatLog chatLog;

    [SerializeField]
    [Label("Match lobby log")]
    [Tab("Interacting scripts")]
    public ChatLog LobbyChatLog;


    [SerializeField]
    [Label("Menu controller")]
    [Tab("Interacting scripts")]
    public MenuController menuController;

    [SerializeField]
    [Label("Network manager")]
    [Tab("Interacting scripts")]
    public NetworkManager network;

    [SerializeField]
    [Label("Offline match maker script")]
    [Tab("Interacting scripts")]
    private OfflineMatchMaker OfflineMatchMaker;

    [SerializeField]
    [Label("Online match maker script")]
    [Tab("Interacting scripts")]
    private OnlineMatchMaker OnlineMatchMaker;

    [SerializeField]
    [Label("Match board")]
    [Tab("Interacting scripts")]
    public MatchBoard matchBoard;

    public enum _ConnectionMode
    {
        Online, Offline
    }
    public static _ConnectionMode ConnectionMode = _ConnectionMode.Offline;


    private IMatchMakerMode matchMakerMode;

    [SerializeField]
    [Tab("GUI elements")]
    private LoadingPanel LoadingPanel;

    [SerializeField]
    [Tab("GUI elements")]
    private GameObject PlayBut;

    void Start()
    {
        LoadingPanel.Close();

        GameObject[] datas = GameObject.FindGameObjectsWithTag("gamedata");
        for(int d = 0; d < datas.Length;d++)
        {
            if(datas[d] != gameData.gameObject)
            {
                Destroy(datas[d].gameObject);
            }
        }

        OfflineMatchMaker.SetMatchMaker(this);
        OnlineMatchMaker.SetMatchMaker(this);

    }

    void Update()
    {
        
    }

    private void SetPlayMode(_ConnectionMode newconnectionMode)
    {
        ConnectionMode = newconnectionMode;
        switch(newconnectionMode)
        {
            case _ConnectionMode.Offline:
                matchMakerMode = OfflineMatchMaker;
                break;
            case _ConnectionMode.Online:
                matchMakerMode = OnlineMatchMaker;
                break;
        }
        
    }

    public void OnPlay()
    {
        matchMakerMode.OnStartMatchMaking();
    }

    public void OnPlayOffline()
    {
        SetPlayMode(_ConnectionMode.Offline);
        LobbyChatLog.Log("You play offline");
        PlayBut.SetActive(true);
    }

    public void OnPlayOnline()
    {
        SetPlayMode(_ConnectionMode.Online);
    }

    public void OnDisconnect()
    {
        matchMakerMode.DisconnectFromRoom();
        matchBoard.HideAllBoards();
        LoadingPanel.Close();
    }
}

interface IMatchMakerMode
{
    void SetMatchMaker(MatchMaker matchmaker);

    void OnStartMatchMaking();

    void DisconnectFromRoom();
}
