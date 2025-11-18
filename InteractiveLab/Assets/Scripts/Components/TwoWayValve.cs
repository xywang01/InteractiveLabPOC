using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayValve : MonoBehaviour
{
    public enum RotationAxis
    {
        Left,
        Forward, 
        Up
    }
    
    public string id;
    public bool open = false;
    public GameObject target;
    public RotationAxis rotationAxis = RotationAxis.Up;
    // public bool rotateVertical;
    // public bool rotateHorizontal;
    public float rotationUnit = 90f;

    private void RotateValve(float angle)
    {
        switch (rotationAxis)
        {
            case RotationAxis.Left:
                transform.RotateAround(target.transform.position, Vector3.left, angle);
                break;
            case RotationAxis.Forward:
                transform.RotateAround(target.transform.position, Vector3.forward, angle);
                break;
            case RotationAxis.Up:
                transform.RotateAround(target.transform.position, Vector3.up, angle);
                break;
        }
    }
    private void Start() {
        if (open) {
            RotateValve(rotationUnit);
        }
    }

    public void TurnValve() {
        open = !open;

        FindObjectOfType<SoundManager>().Play("TurnValve");

        if (open) {
            RotateValve(rotationUnit);
        } else {
            RotateValve(-rotationUnit);
        }
    }

    public void Reset() {
        if (open) {
            RotateValve(rotationUnit);
            open = false;
        }
    }
}
