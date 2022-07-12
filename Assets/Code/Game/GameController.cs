using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using CuriousAttributes;
using AICode;
public class GameController : MonoBehaviourPunCallbacks, IChancelObserver
{
    #region Singletone code

    private static GameController gameController;
    public static GameController Instance
    {
        get { return gameController; }
    }

    #endregion

    #region Observers code

    private List<IGameControllerObserver> observers = new List<IGameControllerObserver>();

    public void AddObserver(IGameControllerObserver newob)
    {
        observers.Add(newob);
        newob.OnGameStateChanged(CurrentGameState);
    }

    public void RemoveObserver(IGameControllerObserver newob)
    {
        observers.Remove(newob);
    }

    private void OnGameStateChangedNotification()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
            {
                observers[i].OnGameStateChanged(CurrentGameState);
            }
            else
            {
                observers.RemoveAt(i);
            }
        }
    }

    #endregion

    public const int GEMS_TO_WIN = 5;

    public GameData gameData { get; private set; }
    private MapInfo mapinfo;

    [Tab("Prefabs")]
    [Prefab]
    [SerializeField]
    private GameObject MyOfflinePlayer;

    [Tab("Prefabs")]
    [Prefab]
    [SerializeField]
    private GameObject OfflinePlayerAI;

    [Tab("Prefabs")]
    [Prefab]
    [SerializeField]
    private GameObject OnlinePlayer;


    [Tab("Controllers")]
    [SerializeField]
    private CameraController cameracontroller;

    [Tab("Controllers")]
    [SerializeField]
    private GameMenuController menucontroller;


    [Tab("GUI elements")]
    [SerializeField]
    private WinGamePanel wingamepanel;


    [Tab("Music")]
    [SerializeField]
    private AudioSource Music;

    [Tab("Music")]
    [SerializeField]
    private AudioClip WinMusic;

    [Tab("Music")]
    [SerializeField]
    private AudioClip FailMusic;

    [Tab("Data")]
    [SerializeField]
    private PathData pathData;

    public int ID { get; private set; }   // index of this client in current room

    public int WinnerID { get; private set; } = -1;

    public enum GameState
    {
        Waiting_for_players,
        Ready,
        Game, 
        Observing_winner
    }

    private GameState gameState = GameState.Waiting_for_players;

    public GameState CurrentGameState
    {
        get { return gameState; }
        set
        {
            gameState = value;
            OnGameStateChangedNotification();
        }
    }


    private List<BasePlayer> allplayers = new List<BasePlayer>();


    public override void OnEnable()
    {
        base.OnEnable();
        gameController = this;
    }

    void Start()
    {      
        if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline)
            StartOffline();
        else
            StartOnline();
    }

    bool wasstart;

    void Update()
    {
        
        if(!wasstart && MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
        {
            ExitGames.Client.Photon.Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;
            table["0"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            bool ready = true;
            for (int i = 0; i < OnlineMatchMaker.playersinroom.Count;i++)
            {
                if((bool)OnlineMatchMaker.playersinroom[i].CustomProperties["0"] == false)
                {
                    ready = false;
                    break;
                }
            }
            if (ready)
            {
                wasstart = true;
                CurrentGameState = GameState.Ready;
                menucontroller.SetAndRunCountDown(PlayOn);
            }
        }
        if(CurrentGameState == GameState.Observing_winner)
        {
            PlayOff();
        }
    }

    private void PlayOn()
    {
        foreach (var p in allplayers)
        {
            p.IsPlaying = true;
        }
        CurrentGameState = GameState.Game;
    }

    private void PlayOff()
    {
        foreach (var p in allplayers)
        {
            p.IsPlaying = false;
        }
    }

    public void StartOffline()
    {
        CurrentGameState = GameState.Waiting_for_players;
        allplayers.Clear();
        // Получение инфы о карте и об игре
        gameData = GameObject.FindWithTag("gamedata").GetComponent<GameData>();
        mapinfo = GameObject.FindWithTag("mapinfo").GetComponent<MapInfo>();

        ID = 0;

        // Спавн игрока
        GameObject player = Instantiate(MyOfflinePlayer) as GameObject;
        Vector2 spawnpos = mapinfo.playersinfo[0].spawnpoint.position; 
        player.transform.position = new Vector3(spawnpos.x, spawnpos.y, 0);
        player.GetComponent<PlayerController>().SetChancel(mapinfo.playersinfo[0].chancel); // Отправка игроку инфы об его алтаре
        BasePlayer basePlayer = player.GetComponent<BasePlayer>();
        allplayers.Add(basePlayer);
        basePlayer.IsPlaying = false;
        basePlayer.SpawnPoint = spawnpos;
        basePlayer.SetName(gameData.playersdata[0].Name);
        basePlayer.SetSkin(gameData.playersdata[0].skin);
        menucontroller.SetTargetPlayer(basePlayer);


        cameracontroller.SetTarget(player.transform); // Отправка камере новой цели для слежки

        
        for(int i = 1; i < gameData.playersdata.Count; i++)
        {
            
            // Спавн ботов
            player = Instantiate(OfflinePlayerAI) as GameObject;
            spawnpos = mapinfo.playersinfo[i].spawnpoint.position;
            player.transform.position = new Vector3(spawnpos.x, spawnpos.y, 0);

            AIOffline AI = player.GetComponent<AIOffline>();
            AI.pathData = pathData;
            AI.SetID(i);

            basePlayer = player.GetComponent<BasePlayer>();
            basePlayer.SetName(gameData.playersdata[i].Name);
            basePlayer.SetSkin(gameData.playersdata[i].skin);
            basePlayer.SpawnPoint = spawnpos;
            basePlayer.IsPlaying = false;
            allplayers.Add(basePlayer);
        }

        for (int i = 0; i < mapinfo.playersinfo.Count; i++)
        {
            PlayerMapInfo info = mapinfo.playersinfo[i];
            // Регистрация у алтарей как их слушатель
            info.chancel.AddObserver(this);
        }
        CurrentGameState = GameState.Ready;
        menucontroller.SetAndRunCountDown(PlayOn);
    }

    public void StartOnline()
    {
        CurrentGameState = GameState.Waiting_for_players;
        allplayers.Clear();
        // Получение инфы о карте и об игре
        gameData = GameObject.FindWithTag("gamedata").GetComponent<GameData>();
        mapinfo = GameObject.FindWithTag("mapinfo").GetComponent<MapInfo>();

        ID = 0;
        for (int i = 0; i < OnlineMatchMaker.playersinroom.Count; i++)
        {
            if (OnlineMatchMaker.playersinroom[i] == PhotonNetwork.LocalPlayer)
            {
                ID = i;
                break;
            }
        }

        Vector2 spawnpos = mapinfo.playersinfo[ID].spawnpoint.position;
        GameObject player = PhotonNetwork.Instantiate(OnlinePlayer.name, new Vector3(spawnpos.x, spawnpos.y, 0),
            Quaternion.identity);
        player.GetComponent<PlayerController>().SetChancel(mapinfo.playersinfo[ID].chancel); // Отправка игроку инфы об его алтаре
        BasePlayer basePlayer = player.GetComponent<BasePlayer>();
        allplayers.Add(basePlayer);
        basePlayer.SetName(gameData.playersdata[ID].Name);
        basePlayer.SetSkin(gameData.playersdata[ID].skin);
        basePlayer.SpawnPoint = spawnpos;
        basePlayer.IsPlaying = false;
        cameracontroller.SetTarget(player.transform);
        player.GetComponent<AudioListener>().enabled = true;

        menucontroller.SetTargetPlayer(basePlayer);

        for (int i = 0; i < mapinfo.playersinfo.Count; i++)
        {
            PlayerMapInfo info = mapinfo.playersinfo[i];

            // Регистрация у алтарей как их слушатель
            info.chancel.AddObserver(this);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
        }
        
    }

    public void OnCollectingAllGems(Chancel chancel)
    {
        PlayOff();
        for(int i = 0; i < mapinfo.numberofplayers;i++)
        {
            allplayers[i].BecomeVisible();
            if(mapinfo.playersinfo[i].chancel == chancel)
            {
                WinnerID = i;
                break;
            }
        }
        cameracontroller.SetTarget(new Vector2(chancel.gameObject.transform.position.x, 
            chancel.gameObject.transform.position.y + 2.77f));
        cameracontroller.ConsiderWithBorders = false;
        cameracontroller.SetCameraSize(5f);

        Music.Stop();
        if (WinnerID == ID)
        {
            Music.clip = WinMusic;
        }
        else
        {
            Music.clip = FailMusic;
        }
        Music.loop = false;
        Music.Play();

        SetPlayersDataToWinPanel();

        CurrentGameState = GameState.Observing_winner;

    }

    private class sortplayerdata
    {
        public int gems;
        public string Name;
        // Если x > y то return -1, Если x < y то return 1, если x == y то return 0 
        public sortplayerdata(int newgems, string newName)
        {
            gems = newgems;
            Name = newName;
        }
        public static int Compare(object x, object y)
        {
            if(((sortplayerdata)x).gems > ((sortplayerdata)y).gems)
            {
                return -1;
            }
            else if (((sortplayerdata)x).gems < ((sortplayerdata)y).gems)
            {
                return 1;
            }
            else if (((sortplayerdata)x).gems == ((sortplayerdata)y).gems)
            {
                return 0;
            }
            return 0;
        }
    }

    private void SetPlayersDataToWinPanel()
    {
        List<string> Names = new List<string>();
        List<int> Amounts = new List<int>();
        List<sortplayerdata> playerslist = new List<sortplayerdata>();
        for(int i = 0; i < gameData.playersdata.Count;i++)
        {
            playerslist.Add(new sortplayerdata(mapinfo.playersinfo[i].chancel.GemAmount(), gameData.playersdata[i].Name));
        }
        bool swapped;
        do
        {
            swapped = false;
            for (int i = 1; i < playerslist.Count; i++)
            {

                if (sortplayerdata.Compare(playerslist[i - 1], playerslist[i]) > 0)
                {
                    sortplayerdata s = playerslist[i - 1];
                    playerslist[i - 1] = playerslist[i];
                    playerslist[i] = s;
                    swapped = true;
                }
            }
        } while (swapped);

        for(int i = 0; i < playerslist.Count;i++)
        {
            Names.Add(playerslist[i].Name);
            Amounts.Add(playerslist[i].gems);
        }

        wingamepanel.SetDataToLists(Names, Amounts);
    }

}

public interface IGameControllerObserver
{
    void OnGameStateChanged(GameController.GameState newgameState);
}
