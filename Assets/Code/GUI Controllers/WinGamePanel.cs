using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using CuriousAttributes;

public class WinGamePanel : MonoBehaviour
{
    [Tab("GUI elements")]
    [SerializeField]
    private Text PlayersListT;

    [Tab("GUI elements")]
    [SerializeField]
    private Text PlayersGemsT;

    [Tab("GUI elements")]
    public Image GhostImg;

    [Tab("GUI elements")]
    public Text WinnerNickname;

    [SerializeField]
    private GameObject[] Gems;

    public void SetDataToLists(List<string> playersnames, List<int> playersgems)
    {
        PlayersListT.text = "";
        PlayersGemsT.text = "";
        foreach(var g in Gems)
        {
            g.SetActive(false);
        }
        for(int i = 0; i < playersnames.Count;i++)
        {
            PlayersListT.text += (i + 1).ToString() + " " + playersnames[i] + "\n";
            PlayersGemsT.text += playersgems[i].ToString() + "\n";
            Gems[i].SetActive(true);
        }
    }

}
