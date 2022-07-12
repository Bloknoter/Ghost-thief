using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PagesData : ScriptableObject
{
    [HideInInspector]
    public List<Page> pages;
    public void OnEnable()
    {
        if (pages == null)
            pages = new List<Page>();
    }
}
