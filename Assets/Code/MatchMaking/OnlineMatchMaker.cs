using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using CuriousAttributes;

public class OnlineMatchMaker : MonoBehaviourPunCallbacks, IMatchMakerMode, IOnEventCallback
{
    [Label("Network Manager")]
    [SerializeField]
    private NetworkManager networkManager;

    [Label("Room chat log")]
    [SerializeField]
    private ChatLog roomChatLog;

    private MatchMaker matchMaker;

    public LoadingPanel LoadingPanel;


    public static List<Player> playersinroom = new List<Player>();

    private int numberofplayers;


    private bool isLoadingScene = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isLoadingScene)
        {
            LoadingPanel.SetProgress(PhotonNetwork.LevelLoadingProgress * 100f);
        }
    }

    private string[] GetMaps(int _numberofplayers)
    {
        return matchMaker.mapsData.GetMapsNames(_numberofplayers);
    }

    #region IMatchMakerMode Interface implementation
    public void SetMatchMaker(MatchMaker matchmaker)
    {
        matchMaker = matchmaker;
    }

    public void OnStartMatchMaking()
    {
        matchMaker.gameData.playersdata.Clear();
        numberofplayers = Random.Range(4, 5);
        networkManager.SetMaxPlayers(numberofplayers);
        PhotonNetwork.AutomaticallySyncScene = true;
        
        networkManager.JoinRandomRoom();
    }

    public void DisconnectFromRoom()
    {
        matchMaker.gameData.ClearAll();
        playersinroom.Clear();
        isLoadingScene = false;
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    private bool wasupdate;
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            matchMaker.gameData.ClearAll();
            playersinroom.Clear();

            playersinroom.Add(PhotonNetwork.LocalPlayer);

            matchMaker.matchBoard.ShowBoard(numberofplayers);
            for (int i = 0; i < numberofplayers; i++)
            {
                matchMaker.gameData.playersdata.Add(new GameData.PlayerData());
            }

            matchMaker.gameData.playersdata[0].Name = PlayerData.Get().Nickname;
            matchMaker.gameData.playersdata[0].skin = PlayerData.Get().Skin;
            matchMaker.matchBoard.ShowPlayer(0, false);
        }
        else
        {
            matchMaker.gameData.ClearAll();
            playersinroom.Clear();

            for (int i = 0; i < PhotonNetwork.PlayerList.Length;i++)
            {
                playersinroom.Add(PhotonNetwork.PlayerList[i]);
            }

            RaiseEventOptions eventoptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
            SendOptions options = new SendOptions { Reliability = true };
            object[] data = new object[2];
            data[0] = PlayerData.Get().Nickname;
            data[1] = PlayerData.Get().GetSkinID();
            PhotonNetwork.RaiseEvent(1, data, eventoptions, options);
        }
    }

    IEnumerator WaitAndLoadGameScene()
    {
        yield return new WaitForSeconds(3f);
        isLoadingScene = true;
        LoadingPanel.Open();
        string[] maps = GetMaps(numberofplayers);
        PhotonNetwork.LoadLevel(maps[Random.Range(0, maps.Length)]);
    }

    private void CreatePocketofPlayersDataAndSendIt()
    {
        object[] result = new object[numberofplayers * 2];
        for (int i = 0; i < numberofplayers; i++)
        {
            int id = i * 2;
            result[id] = matchMaker.gameData.playersdata[i].Name;
            result[id + 1] = matchMaker.skinsData.GetSkinID(matchMaker.gameData.playersdata[i].skin);
        }
        RaiseEventOptions eventoptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions options = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(0, result, eventoptions, options);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playersinroom.Add(newPlayer);
        roomChatLog.Log("Room", $"Player {newPlayer.NickName} has joined!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomChatLog.Log("Room", $"Player {otherPlayer.NickName} has left room");
        if (PhotonNetwork.IsMasterClient)
        {
            int id = 0;
            for (int i = 0; i < playersinroom.Count; i++)
            {
                if (playersinroom[i] == otherPlayer)
                {
                    id = i;
                    break;
                }
            }
            matchMaker.gameData.playersdata[id].SetUndefinedProperties();
            matchMaker.gameData.playersdata.Add(new GameData.PlayerData());
            matchMaker.gameData.playersdata.RemoveAt(id);
            CreatePocketofPlayersDataAndSendIt();
            matchMaker.matchBoard.UpdatePlayers();
        }
        else if (otherPlayer.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.MasterClientId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                int id = 0;
                for (int i = 0; i < playersinroom.Count; i++)
                {
                    if (playersinroom[i] == otherPlayer)
                    {
                        id = i;
                        break;
                    }
                }
                matchMaker.gameData.playersdata[id].SetUndefinedProperties();
                matchMaker.gameData.playersdata.Add(new GameData.PlayerData());
                matchMaker.gameData.playersdata.RemoveAt(id);
                CreatePocketofPlayersDataAndSendIt();
                matchMaker.matchBoard.UpdatePlayers();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch(photonEvent.Code)
        {
            case 0:
                object[] playerdata = (object[])photonEvent.CustomData;
                numberofplayers = playerdata.Length / 2;
                if (matchMaker.gameData.playersdata.Count == 0)
                {
                    for (int i = 0; i < numberofplayers; i++)
                    {
                        matchMaker.gameData.playersdata.Add(new GameData.PlayerData());
                    }
                }
                for (int i = 0; i < numberofplayers;i++)
                {
                    if ((int)playerdata[i * 2 + 1] != -1)
                    {
                        matchMaker.gameData.playersdata[i].Name = (string)playerdata[i * 2];
                        matchMaker.gameData.playersdata[i].skin = matchMaker.skinsData.Skins[(int)playerdata[i * 2 + 1]];
                    }
                }
                matchMaker.matchBoard.ShowBoard(numberofplayers);
                matchMaker.matchBoard.UpdatePlayers();
                break;
            case 1:
                object[] data = (object[])photonEvent.CustomData;
                matchMaker.gameData.playersdata[PhotonNetwork.CurrentRoom.PlayerCount - 1].Name = (string)data[0];
                matchMaker.gameData.playersdata[PhotonNetwork.CurrentRoom.PlayerCount - 1].skin = matchMaker.skinsData.Skins[(int)data[1]];
                if (PhotonNetwork.IsMasterClient)
                {
                    CreatePocketofPlayersDataAndSendIt();
                    if (PhotonNetwork.CurrentRoom.PlayerCount == /*numberofplayers - 1*/2)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                        foreach(var p in playersinroom)
                        {
                            ExitGames.Client.Photon.Hashtable table = p.CustomProperties;
                            table.Add("0", false);
                            p.SetCustomProperties(table);
                        }
                        StartCoroutine(WaitAndLoadGameScene());
                    }
                }
                matchMaker.matchBoard.ShowPlayer(PhotonNetwork.CurrentRoom.PlayerCount - 1);
                
                break;
        }
    }
}
