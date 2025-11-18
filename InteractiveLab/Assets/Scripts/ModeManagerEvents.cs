using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManagerEvents : MonoBehaviour
{
    public delegate void SetMode(TestMode mode);
    public static event SetMode OnSetMode;
    
    private static TestMode _currentMode;
    
    public static void SetTestMode(TestMode mode)
    {
        _currentMode = mode;
        Debug.Log($"Setting test mode to {_currentMode} in ModeManagerEvents");
        OnSetMode?.Invoke(mode);
    }
    
    public static TestMode GetCurrentMode()
    {
        return _currentMode;
    }
}
