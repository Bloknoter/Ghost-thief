using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLog : MonoBehaviour
{
    [SerializeField]
    private int MaxLines;

    [SerializeField]
    private Text LogT;

    [SerializeField]
    [Range(0, 10)]
    private float DeleteTime;

    private List<string> lines = new List<string>();

    void Start()
    {
        
    }

    private bool deleting;

    void Update()
    {
        if (!deleting && lines.Count > 0)
        {
            deleting = true;
            StartCoroutine(DeleteLine());
        }
    }

    public void Log(string message)
    {
        lines.Add(message);
        if(MaxLines > 0)
        {
            if (lines.Count > MaxLines)
            {
                lines.RemoveAt(0);
                RewriteDataToLog();
                return;
            }

        }
        
        LogT.text += message + "\n";
    }

    private IEnumerator DeleteLine()
    {
        lines.RemoveAt(0);
        RewriteDataToLog();
        yield return new WaitForSeconds(DeleteTime);
        deleting = false;
    }

    private void RewriteDataToLog()
    {
        LogT.text = "";
        foreach(var t in lines)
        {
            LogT.text += t + "\n";
        }
    }

}
