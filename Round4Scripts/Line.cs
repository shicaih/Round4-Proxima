using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<LineRenderer>().SetPosition(0, pos1.position);
        GetComponent<LineRenderer>().SetPosition(1, pos2.position);
    }
}
