using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CuriousAttributes;

[CreateAssetMenu(fileName = "Maps Data", menuName = "Create maps data", order = 0)]
[System.Serializable]
public class MapsData : ScriptableObject
{
    [SerializeField]
    [Label("3 players' maps")]
    private List<string> Mapsfor3pl;

    [SerializeField]
    [Label("4 players' maps")]
    private List<string> Mapsfor4pl;

    public string[] GetMapsNames(int number_of_players)
    {
        switch(number_of_players)
        {
            case 3:
                return Mapsfor3pl.ToArray();
                //break;
            case 4:
                return Mapsfor4pl.ToArray();
                //break;
        }
        return Mapsfor3pl.ToArray();
    }
}
