using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CuriousAttributes;

public class MatchBoard : MonoBehaviour
{
    [SerializeField]
    [Label("Players' patterns")]
    public MatchBoardPlayersPatterns playersPatterns;

    [SerializeField]
    [Label("Game data")]
    public GameData gameData;

    private int currentboardID;

    void Start()
    {
        HideAllBoards();
    }

    public void HideAllBoards()
    {
        StopAllCoroutines();
        for (int i = 0; i < playersPatterns.playersPatterns.Count; i++)
        {
            playersPatterns.playersPatterns[i].AllPlayersObject.SetActive(false);
            HideAllPlayers(i);
        }
    }

    public void ShowBoard(int numberofplayers)
    {
        currentboardID = numberofplayers - 3;
        HideAllBoards();
        playersPatterns.playersPatterns[currentboardID].AllPlayersObject.SetActive(true);
    }

    public void ShowPlayer(int playerid, bool showanimation)
    {
        if (showanimation)
            StartCoroutine(showingplayer(playerid));
        else
        {
            playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].GhostNickname.text = gameData.playersdata[playerid].Name;
            playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].GhostImg.color = gameData.playersdata[playerid].skin;
            playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].QuestionImg.SetActive(false);
        }
    }

    public void ShowPlayer(int playerid)
    {
        StartCoroutine(showingplayer(playerid));
    }

    private IEnumerator showingplayer(int id)
    {
        Color color = gameData.playersdata[id].skin;
        UpdatePlayers();
        for (int t = 0; t < 100; t++)
        {
            Color c = playersPatterns.playersPatterns[currentboardID].playerPatterns[id].GhostImg.color;
            c = Color.Lerp(Color.black, color, t / 100f);
            playersPatterns.playersPatterns[currentboardID].playerPatterns[id].GhostImg.color = c;
            yield return new WaitForSeconds(0f);
        }
    }

    public void HidePlayer(int playerid)
    {
        playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].GhostNickname.text = "";
        Color c = Color.black;
        c.a = 1f;
        playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].GhostImg.color = c;
        playersPatterns.playersPatterns[currentboardID].playerPatterns[playerid].QuestionImg.SetActive(true);
    }

    private void HideAllPlayers(int bid)
    {
        for (int i = 0; i < playersPatterns.playersPatterns[bid].playerPatterns.Count; i++)
        {
            playersPatterns.playersPatterns[bid].playerPatterns[i].GhostNickname.text = "";
            Color c = Color.black;
            c.a = 1f;
            playersPatterns.playersPatterns[bid].playerPatterns[i].GhostImg.color = c;
            playersPatterns.playersPatterns[bid].playerPatterns[i].QuestionImg.SetActive(true);
        }
    }

    public void UpdatePlayers()
    {
        for (int i = 0; i < currentboardID + 3; i++)
        {
            if (i < gameData.playersdata.Count)
            {
                if (gameData.playersdata[i].Name != "" && gameData.playersdata[i].Name != null)
                {
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].GhostNickname.text = gameData.playersdata[i].Name;
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].GhostImg.color = gameData.playersdata[i].skin;
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].QuestionImg.SetActive(false);
                }
                else
                {
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].GhostNickname.text = "";
                    Color c = Color.black;
                    c.a = 1f;
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].GhostImg.color = c;
                    playersPatterns.playersPatterns[currentboardID].playerPatterns[i].QuestionImg.SetActive(true);
                }
            }
            else
            {
                break;
            }
        }
    }

}
