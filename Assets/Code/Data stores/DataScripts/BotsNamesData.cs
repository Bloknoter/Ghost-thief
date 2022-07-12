using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CuriousAttributes;

[CreateAssetMenu(fileName = "Bots names Data", menuName = "Create bots names data", order = 0)]
[System.Serializable]
public class BotsNamesData : ScriptableObject
{
    [SerializeField]
    [Label("Bots' names")]
    private List<string> names = new List<string>();
    public string[] GetNames()
    {
        return names.ToArray();
    }
}
