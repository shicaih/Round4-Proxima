using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public List<AudioClip> AudioClips;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(int index)
    {
        GetComponent<AudioSource>().clip = AudioClips[index];
        GetComponent<AudioSource>().Play();
    }
}
