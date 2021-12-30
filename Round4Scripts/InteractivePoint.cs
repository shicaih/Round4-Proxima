using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PointClickEvent: UnityEvent<InteractivePoint>
{

}

public class InteractivePoint : MonoBehaviour
{
    public bool isActivated, isConnected;
    public PointClickEvent PointClick = new PointClickEvent();
    public bool isClicked = false, isEnter = false;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private AudioClip[] audios;
    private Image rend;
    private AudioSource audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        //if(PointClick == null)
        //{
        //    PointClick = new PointClickEvent();
        //}
        isConnected = false;
        isActivated = false;
        rend = GetComponent<Image>();
        audioManager = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClicked)
        {
            Click();
        }

        if (isEnter)
        {
            rend.sprite = sprites[2];
        }
        else if(isActivated)
        {
            rend.sprite = sprites[0];
        }
        else
        {
            rend.sprite = sprites[1];
        }
    }

    public void Click()
    {
        PointClick.Invoke(this);
        if (isActivated)
        {
            audioManager.PlayOneShot(audios[0]);
        }
        else
        {
            audioManager.PlayOneShot(audios[1]);
        }
        isClicked = false;
    }
}
