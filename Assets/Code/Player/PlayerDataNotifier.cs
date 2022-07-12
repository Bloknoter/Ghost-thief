using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataNotifier 
{
    private List<IPlayerDataObserver> observers = new List<IPlayerDataObserver>();

    public void AddObserver(IPlayerDataObserver newob)
    {
        observers.Add(newob);
    }

    public void RemoveObserver(IPlayerDataObserver newob)
    {
        observers.Remove(newob);
    }

    public void OnNicknameChangedNotification(string nickname)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
            {
                observers[i].OnNicknameChanged(nickname);
            }
            else
            {
                observers.RemoveAt(i);
            }
        }
    }

    public void OnNicknameLoadedNotification(string nickname)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
            {
                observers[i].OnNicknameLoaded(nickname);
            }
            else
            {
                observers.RemoveAt(i);
            }
        }
    }

    public void OnSkinChangedNotification(Color skin)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
            {
                observers[i].OnSkinChanged(skin);
            }
            else
            {
                observers.RemoveAt(i);
            }
        }
    }

    public void OnSkinLoadedNotification(Color skin)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] != null)
            {
                observers[i].OnSkinLoaded(skin);
            }
            else
            {
                observers.RemoveAt(i);
            }
        }
    }
}
