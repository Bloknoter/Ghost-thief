using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private List<IGamePanelObserver> observers = new List<IGamePanelObserver>();

    private void Start()
    {

    }

    public void AddObserver(IGamePanelObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IGamePanelObserver observer)
    {
        observers.Remove(observer);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        foreach(var ob in observers)
        {
            ob.OnOpened();
        }
    }

    public void Close()
    {
        foreach (var ob in observers)
        {
            ob.OnClosed();
        }
        gameObject.SetActive(false);
    }

}
