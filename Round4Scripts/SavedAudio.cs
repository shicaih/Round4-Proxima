using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedAudio : MonoBehaviour
{
    public static SavedAudio instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
