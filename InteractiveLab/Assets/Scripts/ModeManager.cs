using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public TestMode testMode;
    public GameObject nonVRObject;
    public GameObject vRObject;
    public GameObject vRPlayer;
    private bool _testModeSet = false;
    
    void Start()
    {
        Debug.Log($"Current test mode: {testMode}");
        
        switch (testMode)
        {
            case TestMode.Screen:
                nonVRObject.SetActive(true);
                vRObject.SetActive(false);
                vRPlayer.SetActive(false);
                break;
            case TestMode.VR:
                nonVRObject.SetActive(false);
                vRObject.SetActive(true);
                vRPlayer.SetActive(true);
                break;
        }
        
        // set the test mode in events that can be shared among other classses
        ModeManagerEvents.SetTestMode(testMode);
        _testModeSet = true;
        Debug.Log("Test mode set");
        
    }

    public void Exit()
    {
        Application.Quit();        
    }
    
    void LateUpdate()
    {
        
    }
    
    
    
    
}
