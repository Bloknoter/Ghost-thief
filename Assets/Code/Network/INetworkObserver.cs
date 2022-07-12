using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkObserver
{
    void OnConnectedToMaster();
    void OnFailedToConnectToMaster();
    void OnJoinedRoom();
}
