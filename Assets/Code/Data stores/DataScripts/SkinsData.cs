using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CuriousAttributes;

[CreateAssetMenu(fileName = "Skins Data", menuName = "Create skins data", order = 0)]
[System.Serializable]
public class SkinsData : ScriptableObject
{
    [SerializeField]
    [Label("Skins")]
    public List<Color> Skins;

    public int GetSkinID(Color skin)
    {
        for (int i = 0; i < Skins.Count; i++)
        {
            if (skin == Skins[i])
                return i;
        }
        return -1;
    }

    public Color Random()
    {
        return Skins[UnityEngine.Random.Range(0, Skins.Count)];
    }

}
