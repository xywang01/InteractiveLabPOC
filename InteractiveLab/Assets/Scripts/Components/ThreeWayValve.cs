using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using System.Text.RegularExpressions;

public enum Position {Top, TopTopRight, TopRight, TopRightRight, Right, BottomRightRight, BottomRight, BottomBottomRight, Bottom, BottomBottomLeft, BottomLeft, BottomLeftLeft, Left, TopLeftLeft, TopLeft, TopTopLeft};
public enum Divisions {halves, quarters, eigths, sixteenths}
public class ThreeWayValve : Valve
{
    public Position position;
    public GameObject target;
    public bool rotateVertical;
    public bool rotateHorizontal;
    public Divisions division = Divisions.quarters;

    public override void TurnValve() {
        string positionString = "top";
        float turnAngle;

        FindObjectOfType<SoundManager>().Play("TurnValve");

        if (division == Divisions.halves)
        {
            position = (Position)(((int)position + 8) % 16);
            positionString = position.ToString();
            turnAngle = -180f;
        } else if (division == Divisions.quarters)
        {
            position = (Position)(((int)position + 4) % 16);
            positionString = position.ToString();
            turnAngle = -90f;
        } else if (division == Divisions.eigths)
        {
            position = (Position)(((int)position + 2) % 16);
            positionString = position.ToString();
            turnAngle = -45f;
        } else
        {
            position = (Position)(((int)position + 1) % 16);
            positionString = position.ToString();
            turnAngle = -22.5f;
        }

        positionString = Regex.Replace(position.ToString(), "(\\B[A-Z])", " $1");
        OutputManagerEvents.RecordToOutput(id, positionString);

        if (rotateVertical) {
            transform.RotateAround(target.transform.position, Vector3.left, turnAngle);
        } else if (rotateHorizontal) {
            transform.RotateAround(target.transform.position, Vector3.forward, turnAngle);
        } else {
            transform.RotateAround(target.transform.position, Vector3.up, turnAngle);
        }
    }

    public void Reset() {
        if (rotateVertical) {
            if (position == Position.Right) {
                transform.RotateAround(target.transform.position, Vector3.left, 270f);
            } else if (position == Position.Bottom) {
                transform.RotateAround(target.transform.position, Vector3.left, 180f);
            } else if (position == Position.Left) {
                transform.RotateAround(target.transform.position, Vector3.left, 90f);
            }
        } else if (rotateHorizontal) {
            if (position == Position.Right) {
                transform.RotateAround(target.transform.position, Vector3.forward, 270f);
            } else if (position == Position.Bottom) {
                transform.RotateAround(target.transform.position, Vector3.forward, 180f);
            } else if (position == Position.Left) {
                transform.RotateAround(target.transform.position, Vector3.forward, 90f);
            }
        } else {
            if (position == Position.Right) {
                transform.RotateAround(target.transform.position, Vector3.up, 270f);
            } else if (position == Position.Bottom) {
                transform.RotateAround(target.transform.position, Vector3.up, 180f);
            } else if (position == Position.Left) {
                transform.RotateAround(target.transform.position, Vector3.up, 90f);
            }
        }

        position = Position.Top;
    }
}
