using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    #region Singletone code

    private static MapInfo mapInfo;
    public static MapInfo Get()
    {
        return mapInfo;
    }

    #endregion

    public int numberofplayers;
    public List<PlayerMapInfo> playersinfo;
    public List<ObjectGroupInfo> mapobjects;

    public List<GemSpawner> gemSpawners;

    public void SetNetworkDatatoAllObjects()
    {
        int size = 0;
        for(int i = 0; i < mapobjects.Count;i++)
        {
            size += mapobjects[i].mapObjects.Count;
        }

        for (int i = 0; i < mapobjects.Count; i++)
        {
            for (int g = 0; g < mapobjects[i].mapObjects.Count; g++)
            {
                mapobjects[i].mapObjects[g].SetID(size);
                size++;
            }
        } 
    }

    private void OnEnable()
    {
        mapInfo = this;
    }

    void Start()
    {
        SetNetworkDatatoAllObjects();
    }
}
