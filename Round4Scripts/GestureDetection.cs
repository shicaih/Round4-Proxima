using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent OnRecognised;
}

public class GestureDetection : MonoBehaviour
{
    public float threshold = 0.1f;
    public OVRCustomSkeleton skeleton;
    public List<Gesture> gestures, releaseGestures;
    private List<OVRBone> fingerBones;
    public bool debugMode = true;
    public bool isGrabbing = false, released = false, onHand = false;
    private Gesture previousGesture;
    public UnityEvent OnGrab, OnRelease;

    // Start is called before the first frame update
    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetGestureData();
        }


        if(RecogniseGestures(gestures) && !onHand)
        {
            //Debug.Log("Is Grabbing");
            OnGrab.Invoke();
        }

        if (RecogniseGestures(releaseGestures) && onHand)
        {
            OnRelease.Invoke();
        }

        if (fingerBones.Count == 0)
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
        }
        
    }
    public bool IsGrabbing
    {
        get { return isGrabbing; }
    }
    public bool Released
    {
        get { return released; }
    }

    void Save()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerDatas = data;
        gestures.Add(g);
    }

    public void GetGestureData()
    {
        Debug.Log("Fire event triggered");
        if (debugMode)
        {
            Save();
        }
    }

    bool RecogniseGestures(List<Gesture> listOfGestures)
    {
        foreach (var gesture in listOfGestures)
        {
            
            bool match = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > threshold)
                {
                    match = false;
                    break;
                }
                match = true;
                
            }

            if (match)
            {
                return true;
            }
        }
        return false;
    }

    Gesture Recognise()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool discarded = false;
            for( int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > threshold)
                {
                    discarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!discarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}
