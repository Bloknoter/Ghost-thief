using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

using CuriousAttributes;

public class NetworkManager : MonoBehaviourPunCallbacks, IPlayerDataObserver
{
    [SerializeField]
    [Label("Lobby chat log")]
    private ChatLog lobbychatlog;

    [SerializeField]
    [Label("Room chat log")]
    private ChatLog roomchatlog;

    [SerializeField]
    private GameObject PlayButton;

    [Label("Current game version")]
    [TextColor(0f, 1f,0f)]
    [BackgroundColor(0f, 1f, 0f)]
    public string version;

    private byte maxPlayers;
    public void SetMaxPlayers(int newnumber)
    {
        maxPlayers = (byte)(Mathf.Clamp(newnumber, 1, 20));
    }

    void Start()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = version;
        PhotonNetwork.PhotonServerSettings.DevRegion = "eu";
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";

        PlayButton.SetActive(false);

        PlayerData.Get().AddObserver(this);

    }

    void Update()
    {
        
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            lobbychatlog.Log("Connecting to the cloud...");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = false;
            MatchMaker.ConnectionMode = MatchMaker._ConnectionMode.Online;
        }
    }

    public override void OnConnectedToMaster()
    {
        lobbychatlog.Log("Server", "You are connected to the cloud");
        PlayButton.SetActive(true);
        lobbychatlog.Log("Server", "rooms count: " + PhotonNetwork.CountOfRooms.ToString());
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        lobbychatlog.Log("The connection failed. Check your Internet connection.");
        lobbychatlog.Log("Problem description: " + debugMessage);
        PlayButton.SetActive(false);
    }

    public void JoinRandomRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            lobbychatlog.Log("Server", "Joining random room...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        lobbychatlog.Log("Server", "No rooms found. Creating new rooom...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayers }, null);
    }

    public override void OnJoinedRoom()
    {
        lobbychatlog.Log("Server", "Joining rhe room is successful");
        roomchatlog.Log("Room", $"Player {PhotonNetwork.NickName} has joined!");
    }

    public override void OnLeftRoom()
    {
        lobbychatlog.Log("Server", "You left the room");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        lobbychatlog.Log("You are disconnected from server");
        PlayButton.SetActive(false);
    }

    #region IPlayerDataObserver implementation
    public void OnNicknameChanged(string newnickname)
    {
        PhotonNetwork.NickName = newnickname;
        lobbychatlog.Log("Server", "Nickname is changed to " + newnickname);
    }

    public void OnSkinChanged(Color newskin)
    {
        
    }

    public void OnNicknameLoaded(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }

    public void OnSkinLoaded(Color skin)
    {
        
    }
    #endregion
}
