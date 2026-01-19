using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using System.Text.RegularExpressions;

public enum Position {Top, TopTopRight, TopRight, TopRightRight, Right, BottomRightRight, BottomRight, BottomBottomRight, Bottom, BottomBottomLeft, BottomLeft, BottomLeftLeft, Left, TopLeftLeft, TopLeft, TopTopLeft};
public enum Divisions {halves, quarters, eigths, sixteenths}
public enum Direction {right, left}
public class ThreeWayValve : Valve
{
    public Position position;
    public GameObject target;
    public bool rotateVertical;
    public bool rotateHorizontal;
    public Divisions division = Divisions.quarters;
    public Direction direction = Direction.right;
    public Direction labelDirection = Direction.right;

    public int maxTurns = 16;
    private int currTurns = -1;
    private Position originalPosition;
    private Quaternion startRotation;

    public void Start()
    {
        originalPosition = position;
    }

    public override void TurnValve() {
        string positionString = "top";
        float turnAngle;
        currTurns++;

        FindObjectOfType<SoundManager>().Play("TurnValve");

        if (division == Divisions.halves)
        {
            position = (Position)(((int)position + (8 * (labelDirection == Direction.left ? -1f : 1f))) % 16);
            positionString = position.ToString();
            turnAngle = 180f * (direction == Direction.right ? -1f : 1f);
        } else if (division == Divisions.quarters)
        {
            position = (Position)(((int)position + (4 * (labelDirection == Direction.left ? -1f : 1f))) % 16);
            positionString = position.ToString();
            turnAngle = 90f * (direction == Direction.right ? -1f : 1f);
        } else if (division == Divisions.eigths)
        {
            position = (Position)(((int)position + (2 * (labelDirection == Direction.left ? -1f : 1f))) % 16);
            positionString = position.ToString();
            turnAngle = 45f * (direction == Direction.right ? -1f : 1f);
        } else
        {
            position = (Position)(((int)position + (1 * (labelDirection == Direction.left ? -1f : 1f))) % 16);
            positionString = position.ToString();
            turnAngle = 22.5f * (direction == Direction.right ? -1f : 1f);
        }

        if (currTurns >= maxTurns)
        {
            currTurns = -1;
            position = originalPosition;
            positionString = position.ToString();
            turnAngle = -1 * ((maxTurns * turnAngle));
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
