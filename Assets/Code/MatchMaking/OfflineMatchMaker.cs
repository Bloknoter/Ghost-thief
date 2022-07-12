using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CuriousAttributes;
using UnityEngine.SceneManagement;

public class OfflineMatchMaker : MonoBehaviour, IMatchMakerMode
{
    private MatchMaker matchMaker;

    [Label("Nicknames for AI Data")]
    public BotsNamesData AINames;
    
    public LoadingPanel LoadingPanel;

    [ExpandGroup("Showing-players' properties")]
    [Label("Time between appearing of bots")]
    [SerializeField]
    private float deltaapperaingtime;

    private AsyncOperation loadingOperation;

    void Start()
    {
        
    }

    void Update()
    {
        if(loadingOperation != null)
        {
            
            LoadingPanel.Open();
            if(loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
                LoadingPanel.SetProgress(99f);
            }
            else
            {
                LoadingPanel.SetProgress(loadingOperation.progress * 100f);
            }
        }
    }

    #region IMatchMakerMode Interface implementation
    public void SetMatchMaker(MatchMaker matchmaker)
    {
        matchMaker = matchmaker;
    }

    public void OnStartMatchMaking()
    {
        StartCoroutine(OfflineMatchMaking());
    }

    public void DisconnectFromRoom()
    {
        matchMaker.gameData.ClearAll();
        launchgame = false;
        loadingOperation = null;
        StopAllCoroutines();
    }
    #endregion

    private string[] GetMaps(int _numberofplayers)
    {
        return matchMaker.mapsData.GetMapsNames(_numberofplayers);
    }

    private bool launchgame; 

    private IEnumerator OfflineMatchMaking()
    {
        launchgame = true;

        int numberofplayers = Random.Range(4, 5);

        matchMaker.gameData.ClearAll();

        string[] maps = GetMaps(numberofplayers);
        yield return new WaitForSeconds(1f);

        matchMaker.matchBoard.ShowBoard(numberofplayers);

        matchMaker.gameData.ClearAll();

        matchMaker.gameData.playersdata.Add(new GameData.PlayerData());

        matchMaker.gameData.playersdata[0].Name = PlayerData.Get().Nickname;
        matchMaker.gameData.playersdata[0].skin = PlayerData.Get().Skin;

        matchMaker.matchBoard.UpdatePlayers();

        for (int i = 1; i < numberofplayers; i++)
        {
            matchMaker.gameData.playersdata.Add(new GameData.PlayerData());
            yield return new WaitForSeconds(deltaapperaingtime);
            string Name = AINames.GetNames()[Random.Range(0, AINames.GetNames().Length)];
            matchMaker.gameData.playersdata[matchMaker.gameData.playersdata.Count - 1].Name = Name;

            matchMaker.gameData.playersdata[matchMaker.gameData.playersdata.Count - 1].skin = matchMaker.skinsData.Random();
            matchMaker.matchBoard.ShowPlayer(matchMaker.gameData.playersdata.Count - 1);
        }

        yield return new WaitForSeconds(deltaapperaingtime);
        if (launchgame)
        {
            LoadingPanel.Open();
            loadingOperation = SceneManager.LoadSceneAsync(maps[Random.Range(0, maps.Length)]);
            loadingOperation.allowSceneActivation = false;
            
        }
    }

}
