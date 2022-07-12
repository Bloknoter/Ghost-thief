using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using CuriousAttributes;

public class PlayerData : MonoBehaviour
{
    #region Singletone
    private static PlayerData playerData;
    public static PlayerData Get()
    {
        return playerData;
    }
    #endregion

    #region Observers code

    public void AddObserver(IPlayerDataObserver newob)
    {
        notifier.AddObserver(newob);
        notifier.OnNicknameLoadedNotification(nickname);
        notifier.OnSkinLoadedNotification(skin);
    }

    public void RemoveObserver(IPlayerDataObserver newob)
    {
        notifier.AddObserver(newob);
    }

    #endregion

    [SerializeField]
    [Label("Skins data")]
    private SkinsData skinsData;

    [Label("Skin-displaying ghost")]
    [SerializeField]
    [ExpandGroup("Displaying data components")]
    private Image ghostImg;

    [Label("Input nickname")]
    [SerializeField]
    [ExpandGroup("Displaying data components")]
    private InputField inputFieldNickname;

    private string nickname;
    public string Nickname
    {
        get { return nickname; }
        set
        {
            if (nickname != value)
            {
                nickname = value;
                SaveData();
                UpdateNicknameDisplaying();
                notifier.OnNicknameChangedNotification(nickname);
            }
        }
    }


    private Color skin;
    public Color Skin
    {
        get 
        {
            return skin;
        }
    }

    private PlayerDataNotifier notifier;

    private void SaveData()
    {
        object[] datalist = new object[2];
        datalist[0] = nickname;
        datalist[1] = GetSkinID();
        SaveLoadCon.SaveData(datalist, "pldat");
    }

    private void LoadData()
    {
        object[] datalist = (object[])SaveLoadCon.LoadData("pldat");
        if(datalist == null)
        {
            nickname = "Default №" + UnityEngine.Random.Range(1, 10000);
            skin = skinsData.Random();
            return;
        }
        nickname = (string)datalist[0];
        if (nickname == null || nickname == "")
            nickname = "Default №" + UnityEngine.Random.Range(1, 10000);

        if (datalist[1] is int && ((int)datalist[1]) > -1 && ((int)datalist[1]) < skinsData.Skins.Count)
        {
            skin = skinsData.Skins[((int)datalist[1])];
        }
        else
        {
            Debug.Log("Deserializing the color has failed");
            skin = skinsData.Random();
        }

        UpdateNicknameDisplaying();
        UpdateSkinDisplaying();

        notifier.OnNicknameLoadedNotification(nickname);
        notifier.OnSkinLoadedNotification(skin);
    }

    public int GetSkinID()
    {
        for (int i = 0; i < skinsData.Skins.Count; i++)
        {
            if (skin == skinsData.Skins[i])
                return i;
        }
        return -1;
    }
    public void SetSkin(int skinid)
    {
        if (skin != skinsData.Skins[skinid])
        {
            skin = skinsData.Skins[skinid];
            SaveData();
            UpdateSkinDisplaying();
            notifier.OnSkinChangedNotification(skin);
        }
    }

    private void UpdateSkinDisplaying()
    {
        ghostImg.color = Skin;
    }

    private void UpdateNicknameDisplaying()
    {
        inputFieldNickname.text = nickname;
    }

    private void OnEnable()
    {
        notifier = new PlayerDataNotifier();
        LoadData();
        playerData = this;
    }

}

public interface IPlayerDataObserver
{
    void OnNicknameLoaded(string nickname);
    void OnSkinLoaded(Color skin);

    void OnNicknameChanged(string newnickname);
    void OnSkinChanged(Color newskin);
}
