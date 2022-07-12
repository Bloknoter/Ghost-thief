using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CuriousAttributes;

[System.Serializable]
public class PlayerPattern : MonoBehaviour
{
    [Label("Image")]
    public Image GhostImg;
    [Label("Text for nickname")]
    public Text GhostNickname;
    [Label("Question icon")]
    public GameObject QuestionImg;
}
