using System;
using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public enum SystemType
    {
        HeatExchange,  // Heat Exchange
        PackedGreen,  // Packed Green
        LeachingSystem,  // Leaching System
    }
    
    public GameObject HeatExchange;
    public GameObject HXProcedure;
    public GameObject HXProcedureVR;
    
    public GameObject PackedGreen;
    public GameObject PDProcedure;
    public GameObject PDProcedureVR;
    
    public GameObject LeachingSystem;
    public GameObject LSProcedure;
    public SystemType activeSystem;
    
    private GameObject _activeHXProcedure;
    private GameObject _activePDProcedure;

    private bool _modeIsSet = false;
    private bool _systemIsActive = false;

    // private void OnEnable()
    // {
    //     ModeManagerEvents.OnSetMode += SetTestMode;
    // }
    //
    // private void OnDisable()
    // {
    //     ModeManagerEvents.OnSetMode -= SetTestMode;
    // }

    void SetTestMode(TestMode testMode)
    {
        Debug.Log($"Current test mode: {testMode} in StateManager");
        switch (testMode)
        {
            case TestMode.Screen:
                _activeHXProcedure = HXProcedure;
                _activePDProcedure = PDProcedure;
                break;
            case TestMode.VR:
                _activeHXProcedure = HXProcedureVR;
                _activePDProcedure = PDProcedureVR;
                break;
        }
        _modeIsSet = true;
    }

    void ActivateSystem(SystemType system)
    {
        if (!_modeIsSet) return;
        
        switch (system)
        {
            case SystemType.HeatExchange:
                Debug.Log("In StateManager: Activating Heat Exchange System");
                HeatExchange.GetComponent<HXSystemState>().enabled = true;
                PackedGreen.GetComponent<PDSystemState>().enabled = false;
                
                _activeHXProcedure.SetActive(true);
                _activePDProcedure.SetActive(false);
                
                break;
            case SystemType.PackedGreen:
                Debug.Log("In StateManager: Activating Packed Green System");
                
                PackedGreen.GetComponent<PDSystemState>().enabled = true;
                HeatExchange.GetComponent<HXSystemState>().enabled = false;
                
                _activeHXProcedure.SetActive(false);
                _activePDProcedure.SetActive(true);
                
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_modeIsSet)
        {
            TestMode testMode = ModeManagerEvents.GetCurrentMode();
            SetTestMode(testMode);
        } else if (_modeIsSet && !_systemIsActive)
        {
            Debug.Log($"Current active system: {activeSystem}");
            ActivateSystem(activeSystem);
        
            OutputManagerEvents.SetSystemType(activeSystem.ToString());
            _systemIsActive = true;
        }
    }

    public void OnChange()
    {
        switch (activeSystem)
        {
            case SystemType.HeatExchange:
                HeatExchange.GetComponent<HXSystemState>().OnChange();
                break;
            case SystemType.PackedGreen:
                PackedGreen.GetComponent<PDSystemState>().OnChange();
                break;
            case SystemType.LeachingSystem:
                LeachingSystem.GetComponent<LeachingSystem>().onChange();
                break;
        }
    }
}
