using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicMove : MonoBehaviour
{
    [SerializeField] private GestureDetection gestureDetection;
    [SerializeField] private bool resetPosition = false;

    private FixedJoint fixedJoint;
    private ConfigurableJoint conJoint;
    public Rigidbody rb, baseRb;
    private Vector3 oriPos;
    private Quaternion oriRotation;
    private bool entered = false;
    private void Awake()
    {
        oriPos = transform.localPosition;
        oriRotation = transform.localRotation;
        //fixedJoint = GetComponent<FixedJoint>();
        //conJoint = GetComponent<ConfigurableJoint>();
        
    }
    private void OnTriggerEnter(Collider collider)
    {
        entered = true;
    }
    private void OnTriggerExit(Collider collider)
    {
        entered = false;
    }

    private void Update()
    {

    }

    public void ResetPosition()
    {
        Debug.Log("ResetPosition");
        Destroy(fixedJoint);
        conJoint = gameObject.AddComponent<ConfigurableJoint>() as ConfigurableJoint;
        SetConJoint(conJoint);
        conJoint.connectedBody = baseRb;
        gestureDetection.onHand = false;
        if (resetPosition)
        {
            transform.localPosition = oriPos;
            transform.localRotation = oriRotation;
        }

    }

    public void AttachRb()
    {
        if (entered)
        {
            if(conJoint != null)
            {
                Destroy(conJoint);
            }

            fixedJoint = gameObject.AddComponent<FixedJoint>() as FixedJoint;
            fixedJoint.connectedBody = rb;
            gestureDetection.onHand = true;
        }
        
    }
    private void SetConJoint(ConfigurableJoint conJoint)
    {
        JointDrive xDrivePara = conJoint.xDrive;
        JointDrive yDrivePara = conJoint.yDrive;
        JointDrive zDrivePara = conJoint.zDrive;
        JointDrive xDriveAngularPara = conJoint.angularXDrive;
        JointDrive yzDriveAngularPara = conJoint.angularYZDrive;

        xDrivePara.positionSpring = 5;
        xDrivePara.positionDamper = 100;
        conJoint.xDrive = xDrivePara;

        yDrivePara.positionSpring = 5;
        yDrivePara.positionDamper = 100;
        conJoint.yDrive = yDrivePara;

        zDrivePara.positionSpring = 5;
        zDrivePara.positionDamper = 100;
        conJoint.zDrive = zDrivePara;

        xDriveAngularPara.positionSpring = 5;
        xDriveAngularPara.positionDamper = 100;
        conJoint.angularXDrive = xDriveAngularPara;

        yzDriveAngularPara.positionSpring = 5;
        yzDriveAngularPara.positionDamper = 100;
        conJoint.angularYZDrive = yzDriveAngularPara;

        conJoint.targetRotation = new Quaternion(90f, 90f, 90f, 1f);

    }
}
