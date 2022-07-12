using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatLog : MonoBehaviour
{
    [SerializeField]
    private Text textfield;


    void Start()
    {
        textfield.text = "";
    }

    void Update()
    {
        
    }

    public void Log(string message)
    {
        Log("", message);
    }

    public void Log(string owner, string message)
    {
        if(owner == null || owner == "")
        {
            textfield.text += "- " + message + "\n";
            return;
        }
        textfield.text += "[" + owner + "]: " + message + "\n";
    }
}
