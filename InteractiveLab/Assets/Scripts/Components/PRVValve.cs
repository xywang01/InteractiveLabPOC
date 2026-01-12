using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public class PRVValve : Valve
{
    public int turn;
    public int maxTurn = 4;
    public bool overrideToFlip = false;
    public GameObject target;
    public bool rotateVertical;
    public bool rotateHorizontal;

    public void TurnValve(string direction) {
        OutputManagerEvents.RecordToOutput(id, $"Turn {turn.ToString()}");
        FindObjectOfType<SoundManager>().Play("TurnCircleValve");
        float turnAngle;

        if (overrideToFlip)
        {
            turnAngle = 180f * (direction == "right" ? -1f : 1f);
        }
        else
        {
            turnAngle = (360f / maxTurn) * (direction == "right" ? -1f : 1f);
        }

        if (direction == "left" && turn > 0) {
            turn--;
        }

        if (direction == "right" && turn < maxTurn) {
            turn++;
        }

        if (direction == "right" && turn >= maxTurn) {
            turn = 0;
        }

        if (target != null)
        {
            if (rotateVertical)
            {
                transform.RotateAround(target.transform.position, Vector3.left, turnAngle);
            }
            else if (rotateHorizontal)
            {
                transform.RotateAround(target.transform.position, Vector3.forward, turnAngle);
            }
            else
            {
                transform.RotateAround(target.transform.position, Vector3.up, turnAngle);
            }
        }
    }

    public void Reset() {
        turn = 0;
    }
}
