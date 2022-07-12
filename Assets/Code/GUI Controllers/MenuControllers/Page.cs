using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Page : ScriptableObject
{
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public int objectindex;
    [HideInInspector]
    public List<string> transitions;
}
