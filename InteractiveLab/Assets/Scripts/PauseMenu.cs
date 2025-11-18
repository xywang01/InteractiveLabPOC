using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseMenuUI;
    // public GameObject vrCanvas;
    
    [SerializeField]
    private XRNode xrNode = XRNode.RightHand;
    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice device;
    private bool wasPressed = false;

    private Vector3 _initialPosition, _initialRotation;
    private bool _atOriginalLoc = true;
    private Vector3 _upstairsLoc = new Vector3(4f,3f, -7f);
    private Vector3 _upstairsRot = new Vector3(0f, 90f, 0f);
    private bool _changeLocPressed = false;

    private void Start()
    {
        Transform thisTransform = transform;
        _initialPosition = thisTransform.position;
        _initialRotation = thisTransform.eulerAngles;
    }

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNode, devices);
        device = devices.FirstOrDefault();
    }

    // Update is called once per frame
    void Update()
    {
        // it turns out for VR, Canvas appears twice - one is VR Canvas (Camera) and the other is VR Canvas (World
        // Space). This caused the button press to be registered twice in a row.
        
        if (!device.isValid)
        {
            GetDevice();
        }
        
        // check for pause menu
        bool isPressed = device.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed) && pressed;

        if (!isPressed) {
            wasPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab) || (isPressed && !wasPressed)) {
            wasPressed = true;

            if (paused) {
                Resume();
            } else {
                Pause();
            }
        }
        
        // ability to move the pause menu in VR to upstairs
        if (paused)
        {
            bool changeLoc = device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool changeLocPressed) && changeLocPressed;
            if (!changeLoc)
                _changeLocPressed = false;
            
            if (changeLoc && !_changeLocPressed)
            {
                Transform thisTransform = transform;
                _changeLocPressed = true;
                if (_atOriginalLoc)
                {
                    
                    thisTransform.position = _upstairsLoc + _initialPosition;
                    thisTransform.eulerAngles = _upstairsRot;
                    _atOriginalLoc = false;
                }
                else
                {
                    thisTransform.position = _initialPosition;
                    thisTransform.eulerAngles = _initialRotation;
                    _atOriginalLoc = true;
                }
            }
        }
    }

    void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Debug.Log($"Pause menu is inactive, Paused is {paused}");
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        // Debug.Log($"Pause menu is active, Paused is {paused}");
    }

    public void Exit() {
        Application.Quit();
    }
}
