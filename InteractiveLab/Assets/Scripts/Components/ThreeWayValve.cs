using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public enum Position {top, left, bottom, right};
public class ThreeWayValve : Valve
{
    public Position position;
    public GameObject target;
    public bool rotateVertical;
    public bool rotateHorizontal;

    public override void TurnValve() {
        string positionString = "top";

        FindObjectOfType<SoundManager>().Play("TurnValve");


        if (position == Position.top) {
            position = Position.right;
            positionString = "right";
        } else if (position == Position.right) {
            position = Position.bottom;
            positionString = "bottom";
        } else if (position == Position.bottom) {
            position = Position.left;
            positionString = "left";
        } else {
            position = Position.top;
        }

        OutputManagerEvents.RecordToOutput(id, positionString);

        if (rotateVertical) {
            transform.RotateAround(target.transform.position, Vector3.left, 90f);
        } else if (rotateHorizontal) {
            transform.RotateAround(target.transform.position, Vector3.forward, 90f);
        } else {
            transform.RotateAround(target.transform.position, Vector3.up, 90f);
        }
    }

    public void Reset() {
        if (rotateVertical) {
            if (position == Position.right) {
                transform.RotateAround(target.transform.position, Vector3.left, 270f);
            } else if (position == Position.bottom) {
                transform.RotateAround(target.transform.position, Vector3.left, 180f);
            } else if (position == Position.left) {
                transform.RotateAround(target.transform.position, Vector3.left, 90f);
            }
        } else if (rotateHorizontal) {
            if (position == Position.right) {
                transform.RotateAround(target.transform.position, Vector3.forward, 270f);
            } else if (position == Position.bottom) {
                transform.RotateAround(target.transform.position, Vector3.forward, 180f);
            } else if (position == Position.left) {
                transform.RotateAround(target.transform.position, Vector3.forward, 90f);
            }
        } else {
            if (position == Position.right) {
                transform.RotateAround(target.transform.position, Vector3.up, 270f);
            } else if (position == Position.bottom) {
                transform.RotateAround(target.transform.position, Vector3.up, 180f);
            } else if (position == Position.left) {
                transform.RotateAround(target.transform.position, Vector3.up, 90f);
            }
        }

        position = Position.top;
    }
}
