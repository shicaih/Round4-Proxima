using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicInput : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRecording(int seconds)
    {
        StartCoroutine("Record", seconds);
    }

    IEnumerator Record(int seconds)
    {
        SavedAudio.instance.GetComponent<AudioSource>().clip = Microphone.Start(Microphone.devices[0], true, seconds, 44100);
        yield return new WaitForSeconds(seconds);
        Microphone.End(Microphone.devices[0]);

    }
}
