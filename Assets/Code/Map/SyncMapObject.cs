using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SyncMapObject : MonoBehaviourPunCallbacks, IOnEventCallback
{

    private int ID = 0;
    public void SetID(int newID) { ID = newID; }

    public void SetCallBack(UnityAction<object> action)
    {
        callback = action;
    }

    private UnityAction<object> callback;

    public void SendData(object senddata)
    {
        RaiseEventOptions eventoptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendoptions = new SendOptions { Reliability = true };
        object[] data = new object[2];
        data[0] = ID;
        data[1] = senddata;
        PhotonNetwork.RaiseEvent(100, data, eventoptions, sendoptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            //    data : object[i] ;  i == 1 : object ID
            //                        i == 2 : data

            case 100:
                object[] data = (object[])photonEvent.CustomData;
                if((int)data[0] == ID)
                {
                    callback(data[1]);
                }
                break;
        }
    }
}
