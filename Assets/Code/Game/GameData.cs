using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData : MonoBehaviour
{

    public List<PlayerData> playersdata = new List<PlayerData>();

    public class PlayerData
    {
        public string Name;
        public Color skin;
        public void SetUndefinedProperties()
        {
            Name = "";
            Color c = Color.black;
            c.a = 1f;
            skin = c;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ClearAll()
    {
        playersdata.Clear();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
