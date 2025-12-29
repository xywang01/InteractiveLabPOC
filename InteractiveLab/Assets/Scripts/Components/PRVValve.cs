using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public class PRVValve : Valve
{
    public int turn;
    public bool rotateVertical;
    public bool rotateHorizontal;

    public void TurnValve(string direction) {
        OutputManagerEvents.RecordToOutput(id, $"Turn {turn.ToString()}");
        FindObjectOfType<SoundManager>().Play("TurnCircleValve");

        if (direction == "left" && turn > 0) {
            turn--;
        }

        if (direction == "right" && turn < 4) {
            turn++;
        }

        if (direction == "right" && turn >= 4) {
            turn = 0;
        }
    }

    public void Reset() {
        turn = 0;
    }
}
