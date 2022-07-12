using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

using CuriousAttributes;

public class GameMenuController : MonoBehaviourPunCallbacks, IGameControllerObserver
{
    public delegate void _Callback();


    [Tab("Panels")]
    [SerializeField]
    private GamePanel PlayerPanel;

    [Tab("Panels")]
    [SerializeField]
    private GamePanel EndGamePanel;

    [Tab("Panels")]
    [SerializeField]
    private WinGamePanel WinnerGamePanel;

    [Tab("GUI elements")]
    [SerializeField]
    private Text WaitingforPlayersT;

    [Tab("GUI elements")]
    [SerializeField]
    private Text CountDownT;


    [Tab("GUI elements")]
    [SerializeField]
    private Text NickNameT;

    [Tab("GUI elements")]
    [SerializeField]
    private Image Skin;


    [Tab("GUI elements")]
    [SerializeField]
    private Slider EnergySl;

    [Tab("GUI elements")]
    [SerializeField]
    private Text EnergyT;


    [Tab("GUI elements")]
    [SerializeField]
    private GameLog gamelog;

    [Tab("GUI elements")]
    [SerializeField]
    private LoadingPanel loadingPanel;


    [Tab("Resources")]
    [SerializeField]
    private Sprite GhostSprite;

    private BasePlayer targetplayer;

    private bool isloadinglobby;

    private AsyncOperation loadingOperation;

    void Start()
    {
        GameController.Instance.AddObserver(this);

        WaitingforPlayersT.text = "Waiting for players...";

        EnergySl.maxValue = BasePlayer.MAX_ENERGY;

        EndGamePanel.Close();
    }

    void Update()
    {
        if (targetplayer != null)
        {
            EnergySl.value = targetplayer.GetEnergy();
            EnergyT.text = targetplayer.GetEnergy().ToString();
        }
        if(GameController.Instance.CurrentGameState == GameController.GameState.Waiting_for_players)
        {
            if(!wasanimated)
            {
                wasanimated = true;
                StartCoroutine(AnimatingWaitingforplayers());
            }
        }
        if(isloadinglobby)
        {
            loadingPanel.Open();
            if(MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline)
            {
                if(loadingOperation != null)
                {
                    if(loadingOperation.progress >= 0.9f)
                    {
                        loadingPanel.SetProgress(99f);
                        loadingOperation.allowSceneActivation = true;         
                    }
                    else
                    {
                        loadingPanel.SetProgress(loadingOperation.progress * 100f);
                    }
                }
            }
            if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
            {
                loadingPanel.SetProgress(PhotonNetwork.LevelLoadingProgress * 100f);
            }
        }
    }

    public void SetTargetPlayer(BasePlayer newTarget)
    {
        targetplayer = newTarget;
        Skin.color = targetplayer.GetSkin();
        NickNameT.text = targetplayer.GetName();
    }

    public void SetAndRunCountDown(_Callback endcallback)
    {
        StartCoroutine(CountDown(endcallback));
    }

    private bool wasanimated;
    private IEnumerator AnimatingWaitingforplayers()
    {
        WaitingforPlayersT.text = "Waiting for players...";
        yield return new WaitForSeconds(0.7f);
        WaitingforPlayersT.text = "Waiting for players.";
        yield return new WaitForSeconds(0.7f);
        WaitingforPlayersT.text = "Waiting for players..";
        yield return new WaitForSeconds(0.7f);
        WaitingforPlayersT.text = "Waiting for players...";
        wasanimated = false;
    }

    private IEnumerator CountDown(_Callback callback)
    {
        CountDownT.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            CountDownT.text = (3 - i).ToString();
            yield return new WaitForSeconds(1f);
        }
        CountDownT.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        gamelog.Log("Sorry, but you are disconnected from server :(");
    }

    public override void OnLeftRoom()
    {
        gamelog.Log("Sorry, but you have left the game :(");
    }

    public void LoadLobby()
    {
        isloadinglobby = true;
        loadingPanel.Open();
        if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Online)
            PhotonNetwork.LeaveRoom();
        if (MatchMaker.ConnectionMode == MatchMaker._ConnectionMode.Offline)
        {
            
            loadingOperation = SceneManager.LoadSceneAsync("Lobby");
            loadingOperation.allowSceneActivation = false;
        }
    }

    #region IGameControllerObserver interface implementation

    public void OnGameStateChanged(GameController.GameState newgameState)
    {
        if(newgameState == GameController.GameState.Waiting_for_players)
        {
            PlayerPanel.Close();
            WaitingforPlayersT.gameObject.SetActive(true);
            CountDownT.gameObject.SetActive(false);
        }
        if (newgameState == GameController.GameState.Ready)
        {
            PlayerPanel.Open();
            WaitingforPlayersT.gameObject.SetActive(false);
            CountDownT.gameObject.SetActive(true);
        }
        if(newgameState == GameController.GameState.Game)
        {
            WaitingforPlayersT.gameObject.SetActive(false);
            CountDownT.gameObject.SetActive(false);
        }
        if (newgameState == GameController.GameState.Observing_winner)
        {
            PlayerPanel.Close();
            EndGamePanel.Open();
            WinnerGamePanel.GhostImg.sprite = GhostSprite;
            int ID = GameController.Instance.ID;
            int WinnerID = GameController.Instance.WinnerID;
            WinnerGamePanel.GhostImg.color = GameController.Instance.gameData.playersdata[WinnerID].skin;
            if (WinnerID != ID)
                WinnerGamePanel.WinnerNickname.text = "Player " + GameController.Instance.gameData.playersdata[WinnerID].Name + " wins!";
            else
                WinnerGamePanel.WinnerNickname.text = "You win!";
        }
    }

    #endregion
}
