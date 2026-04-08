using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using UnityEngine.XR;

public class Valve : MonoBehaviour
{
    public string id;
    public bool open = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void ResetTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTransform();
        }
        else
        {
            InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            bool primaryButtonPressed;
            if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed) && primaryButtonPressed)
            {
                ResetTransform();
            }
        }
    }

    public virtual void TurnValve()
    {
        OutputManagerEvents.RecordToOutput(id, open ? "Open" : "Close");
    }
}
