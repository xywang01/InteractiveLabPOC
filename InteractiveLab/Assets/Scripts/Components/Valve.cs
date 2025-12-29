using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public class Valve : MonoBehaviour
{
    public string id;
    public bool open = false;

    public virtual void TurnValve()
    {
        OutputManagerEvents.RecordToOutput(id, open ? "Open" : "Close");
    }
}
