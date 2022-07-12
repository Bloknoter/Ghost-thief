using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapInfo))]
[CanEditMultipleObjects]
public class MapInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapInfo info = (MapInfo)target;

        if (info.gemSpawners == null)
        {
            info.gemSpawners = new List<GemSpawner>();

        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Гем спавнеры");
        if (GUILayout.Button("Добавить", GUILayout.Width(100)))
        {
            info.gemSpawners.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < info.gemSpawners.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            info.gemSpawners[i] = (GemSpawner)EditorGUILayout.ObjectField($"Гем спавнер {i+1}", info.gemSpawners[i], typeof(GemSpawner), true);  
            if (GUILayout.Button("Удалить", GUILayout.Width(80)))
            {
                info.gemSpawners.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("===========================================================================================");

        info.numberofplayers = EditorGUILayout.IntSlider("Кол-во игроков", info.numberofplayers, 3, 6);
        
        if(info.playersinfo == null)
        {
            info.playersinfo = new List<PlayerMapInfo>();
            for(int i = 0; i < info.numberofplayers;i++)
            {
                info.playersinfo.Add((PlayerMapInfo)CreateInstance("PlayerMapInfo"));
            }
                   
        }

        if(info.playersinfo.Count < info.numberofplayers)
        {
            for (int i = 0; i < info.numberofplayers - info.playersinfo.Count; i++)
            {
                info.playersinfo.Add((PlayerMapInfo)CreateInstance("PlayerMapInfo"));
            }
        }
        else if(info.playersinfo.Count > info.numberofplayers)
        {
            for (int i = 0; i < info.playersinfo.Count - info.numberofplayers; i++)
            {
                info.playersinfo.RemoveAt(info.playersinfo.Count - 1);
            }
        }


        EditorGUILayout.LabelField("===========================================================================================");

        for (int i = 0; i < info.playersinfo.Count; i++)
        {
            PlayerMapInfo currinfo = info.playersinfo[i];

            if (currinfo != null)
            {
                EditorGUILayout.LabelField("Игрок " + (i + 1).ToString());

                currinfo.spawnpoint = (Transform)EditorGUILayout.ObjectField("Спавн", currinfo.spawnpoint, typeof(Transform), true);
                currinfo.chancel = (Chancel)EditorGUILayout.ObjectField("Алтарь", currinfo.chancel, typeof(Chancel), true);
            }
            else
            {
                info.playersinfo.RemoveAt(i);
            }
            EditorGUILayout.LabelField("===========================================================================================");
        }


        if (info.mapobjects == null)
            info.mapobjects = new List<ObjectGroupInfo>();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Объекты для синхронизации по сети");
        if (GUILayout.Button("Добавить группу", GUILayout.Width(130)))
        {
            info.mapobjects.Add(new ObjectGroupInfo());
            info.mapobjects[info.mapobjects.Count - 1].mapObjects = new List<SyncMapObject>();
            info.mapobjects[info.mapobjects.Count - 1].mapObjects.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < info.mapobjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Группа " + (i + 1).ToString());
            if (GUILayout.Button("Удалить", GUILayout.Width(80)))
            {
                info.mapobjects.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Добавить объект", GUILayout.Width(150)))
            {
                info.mapobjects[i].mapObjects.Add(null);
            }

            for (int g = 0; g < info.mapobjects[i].mapObjects.Count; g++)
            {
                EditorGUILayout.BeginHorizontal();
                info.mapobjects[i].mapObjects[g] = (SyncMapObject)EditorGUILayout.ObjectField("Объект " + g.ToString(),
                    info.mapobjects[i].mapObjects[g], typeof(SyncMapObject), true);
                if (GUILayout.Button("Удалить", GUILayout.Width(80)))
                {
                    info.mapobjects[i].mapObjects.RemoveAt(g);
                }
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.LabelField("===========================================================================================");

        }

        EditorUtility.SetDirty(info);
    }
}
