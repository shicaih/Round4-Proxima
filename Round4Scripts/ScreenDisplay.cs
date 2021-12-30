using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDisplay : MonoBehaviour
{
    public Vector3 DefaultScale;
    Vector3 TargetScale;

    public bool TurnedOn = false;


    void Start()
    {
        DefaultScale = new Vector3(1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().localScale += new Vector3((TargetScale.x - GetComponent<RectTransform>().localScale.x) / 10, (TargetScale.y - GetComponent<RectTransform>().localScale.y) / 30);
    }

    public void TurnOn()
    {
        TurnedOn = true;
        TargetScale = DefaultScale;
    }

    public void TurnOff()
    {
        TurnedOn = false;
        TargetScale = new Vector3(0, 0, 0);
    }
}
