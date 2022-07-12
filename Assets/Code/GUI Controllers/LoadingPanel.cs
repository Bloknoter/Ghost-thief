using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private Slider loadingSl;

    [SerializeField]
    private Text loadingT;

    private float progress = 0f;

    void Start()
    {
        loadingSl.maxValue = 100;
        loadingSl.value = 0;
        loadingT.text = "0%";
        gameObject.SetActive(false);
    }

    void Update()
    {
        loadingSl.value = (int)Mathf.Lerp(loadingSl.value, progress, 0.5f);
        loadingT.text = $"{(int)progress}%";
    }

    public void SetProgress(float newprogress)
    {
        progress = Mathf.Clamp(newprogress, 0, 100);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
