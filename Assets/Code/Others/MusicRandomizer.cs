using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRandomizer : MonoBehaviour
{

    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private AudioSource MusicSource;

    void Start()
    {
        MusicSource.Stop();
        MusicSource.clip = clips[Random.Range(0, clips.Length)];
        MusicSource.loop = true;
        MusicSource.Play();
    }

}
