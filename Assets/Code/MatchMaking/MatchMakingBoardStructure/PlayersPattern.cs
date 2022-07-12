using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CuriousAttributes;

[System.Serializable]
public class PlayersPattern : MonoBehaviour
{
    [Range(3, 5)]
    [TextColor(0.69f, 0.1f, 0.02f)]
    [Label("Number of players")]
    public int numberofplayers;

    [Label("Container for all objects")]
    public GameObject AllPlayersObject;
    [SerializeField]
    [Label("Players visual info")]
    public List<PlayerPattern> playerPatterns;

}
