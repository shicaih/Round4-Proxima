using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class MainButton : MonoBehaviour
{
    public UnityEvent onPressed, onReleased;
    [SerializeField] float threshold = 0.1f;
    [SerializeField] float deadZone = 0.025f;

    private bool isPressed = false;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    private void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();

    }

    private void Update()
    {
        if (!isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        if (isPressed && GetValue() - threshold <= 0)
        {
            Released();
        }

    }

    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        if(Mathf.Abs(value) < deadZone)
        {
            value = 0;
        }
        return Mathf.Clamp(value, -1f, 1f);
    }
    private void Pressed()
    {
        isPressed = true;
        onPressed.Invoke();
        Debug.Log("Pressed");
    }
    private void Released()
    {
        isPressed = false;
        onReleased.Invoke();
        Debug.Log("Released");
    }
}
