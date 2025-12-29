using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleValve : Valve
{
    public bool rotateVertical;
    public bool rotateHorizontal;

    public override void TurnValve() {
        base.TurnValve();

        FindObjectOfType<SoundManager>().Play("TurnCircleValve");
        open = !open;
        
        Debug.Log($"valve {id} is turned and is now {open}");
    }

    public void Reset() {
        open = false;
    }
}
