using System;
using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string twoWayValveTag = "TwoWayValve";
    [SerializeField] private string threeWayValveTag = "ThreeWayValve";
    [SerializeField] private string circleValveTag = "CircleValve";
    [SerializeField] private string condensationTrapTag = "CondensationTrap";
    [SerializeField] private string PRVTag = "PRV";
    [SerializeField] private string ComputerTag = "Computer";
    [SerializeField] private string InfoGaugeTag = "InfoGauge";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    public float distanceToSee;

    public Transform leftRayInteractor;
    public Transform rightRayInteractor;
    private bool leftPrevPressed = false;
    private bool rightPrevPressed = false;

    public Text interactCaption;
    public Image captionBackground;

    public Text interactCaptionVR;
    public Image captionBackgroundVR;

    private Text _interactCaptionActive;
    private Image _captionBackgroundActive;

    public GameObject videoPanel;
    public GameObject DeltaV;
    public GameObject stateManager;
    private Transform _selection;

    private TestMode _testMode;
    private bool _modeIsSet;

    private void OnEnable()
    {
    }

    private void Start()
    {
        XRGrabInteractable[] allGrabbables = FindObjectsOfType<XRGrabInteractable>();

        foreach (XRGrabInteractable grab in allGrabbables)
        {
            grab.enabled = false;
        }
    }

    private void SetTestMode(TestMode mode)
    {
        switch (mode)
        {
            case TestMode.Screen:
                _interactCaptionActive = interactCaption;
                _captionBackgroundActive = captionBackground;
                break;
            case TestMode.VR:
                _interactCaptionActive = interactCaptionVR;
                _captionBackgroundActive = captionBackgroundVR;
                break;
        }

        _interactCaptionActive.text = "";

        Debug.Log($"TestMode is set {mode} in SelectionManager");
    }

    private void Update()
    {
        if (!_modeIsSet)
        {
            _testMode = ModeManagerEvents.GetCurrentMode();
            SetTestMode(_testMode);
            _modeIsSet = true;
        }

        if (Input.GetKeyDown("t"))
        {
            videoPanel.SetActive(!videoPanel.activeSelf);
        }

        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = defaultMaterial;  // Reset material to default
            }
            _selection = null;  // Clear the current selection
            _interactCaptionActive.text = "";  // Clear the interaction caption
            _captionBackgroundActive.enabled = false;  // Disable the caption background
        }

        // todo check if I can get raycast from joystick controller
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (_testMode == TestMode.VR)
        {
            Ray rightRay = new Ray(rightRayInteractor.position, rightRayInteractor.forward);
            Ray leftRay = new Ray(leftRayInteractor.position, leftRayInteractor.forward);
            if (Physics.Raycast(rightRay, out hit, distanceToSee) && !PauseMenu.paused)
            {
                Select(hit);
            }
            else if (Physics.Raycast(leftRay, out hit, distanceToSee) && !PauseMenu.paused)
            {
                Select(hit);
            }
            return;
        }

        if (Physics.Raycast(ray, out hit, distanceToSee) && !PauseMenu.paused)
        {
            Select(hit);
        }
    }

    private void Select(RaycastHit hit)
    {
        var selection = hit.transform;

        // Debug.Log($"selection tag is {selection.tag}");

        var selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            selectionRenderer.material = highlightMaterial;
        }
        _selection = selection;

        if (selection.CompareTag(InfoGaugeTag))
        {
            _interactCaptionActive.text = hit.collider.gameObject.GetComponent<InfoGauge>().description + hit.collider.gameObject.GetComponent<InfoGauge>().value;
            _captionBackgroundActive.enabled = true;
            Debug.Log("selection tag is info gauge");
        }

        if (selection.CompareTag(twoWayValveTag))
        {
            var action = "";
            if (hit.collider.gameObject.GetComponent<TwoWayValve>().open)
            {
                action = "Close ";
            }
            else
            {
                action = "Open ";
            }
            _interactCaptionActive.text = action + hit.collider.gameObject.GetComponent<TwoWayValve>().id + " [Left Click]";
            _captionBackgroundActive.enabled = true;
            Debug.Log("selection tag is two way valve");

            if (_testMode == TestMode.Screen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var target = hit.collider.gameObject.GetComponent<TwoWayValve>();
                    Debug.Log("Hit " + target.id);
                    target.TurnValve();
                    stateManager.GetComponent<StateManager>().OnChange();

                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, target.open ? "Open" : "Close");
                }
            }
            else
            {
                InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                bool leftPressed = false;
                bool rightPressed = false;

                leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftPressed);
                rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightPressed);

                bool leftJustPressed = leftPressed && !leftPrevPressed;
                bool rightJustPressed = rightPressed && !rightPrevPressed;

                if (leftJustPressed || rightJustPressed)
                {
                    var target = hit.collider.gameObject.GetComponent<TwoWayValve>();
                    if (target != null)
                    {
                        Debug.Log("Hit " + target.id);
                        target.TurnValve();
                        stateManager.GetComponent<StateManager>().OnChange();
                    }
                }

                leftPrevPressed = leftPressed;
                rightPrevPressed = rightPressed;
            }
        }

        if (selection.CompareTag(threeWayValveTag))
        {
            _interactCaptionActive.text = "Turn " + hit.collider.gameObject.GetComponent<ThreeWayValve>().id + "(" + hit.collider.gameObject.GetComponent<ThreeWayValve>().position + ") [Left Click]";
            _captionBackgroundActive.enabled = true;
            Debug.Log("selection tag is three way valve");


            if (_testMode == TestMode.Screen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var target = hit.collider.gameObject.GetComponent<ThreeWayValve>();
                    Debug.Log("Hit " + target.id);
                    target.TurnValve();
                    stateManager.GetComponent<StateManager>().OnChange();

                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, target.position.ToString());
                }
            }
            else
            {
                InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                bool leftPressed = false;
                bool rightPressed = false;

                leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftPressed);
                rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightPressed);

                bool leftJustPressed = leftPressed && !leftPrevPressed;
                bool rightJustPressed = rightPressed && !rightPrevPressed;

                if (leftJustPressed || rightJustPressed)
                {
                    var target = hit.collider.gameObject.GetComponent<ThreeWayValve>();
                    if (target != null)
                    {
                        Debug.Log("Hit " + target.id);
                        target.TurnValve();
                        stateManager.GetComponent<StateManager>().OnChange();
                    }
                }

                leftPrevPressed = leftPressed;
                rightPrevPressed = rightPressed;
            }

            if (selection.CompareTag(circleValveTag))
            {
                var action = "";
                if (hit.collider.gameObject.GetComponent<CircleValve>().open)
                {
                    action = "Close ";
                }
                else
                {
                    action = "Open ";
                }
                _interactCaptionActive.text = action + hit.collider.gameObject.GetComponent<CircleValve>().id + " [Left Click]";
                _captionBackgroundActive.enabled = true;
                Debug.Log("selection tag is circle valve");

                if (_testMode == TestMode.Screen)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var target = hit.collider.gameObject.GetComponent<CircleValve>();
                        Debug.Log("Hit " + target.id);
                        target.TurnValve();
                        stateManager.GetComponent<StateManager>().OnChange();
                    }
                }
                else
                {
                    InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                    InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                    bool leftPressed = false;
                    bool rightPressed = false;

                    leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftPressed);
                    rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightPressed);

                    bool leftJustPressed = leftPressed && !leftPrevPressed;
                    bool rightJustPressed = rightPressed && !rightPrevPressed;

                    if (leftJustPressed || rightJustPressed)
                    {
                        var target = hit.collider.gameObject.GetComponent<CircleValve>();
                        if (target != null)
                        {
                            target.TurnValve();
                            stateManager.GetComponent<StateManager>().OnChange();
                        }
                    }

                    leftPrevPressed = leftPressed;
                    rightPrevPressed = rightPressed;
                }
            }

            if (selection.CompareTag(condensationTrapTag))
            {
                _interactCaptionActive.text = "Condensation Trap Liquid Level: " + hit.collider.gameObject.GetComponent<CondensationTrap>().liquidLevel + "%";
                _captionBackgroundActive.enabled = true;
            }

            if (selection.CompareTag(PRVTag))
            {
                _interactCaptionActive.text = "Turn " + hit.collider.gameObject.GetComponent<PRVValve>().id + "(current: " + hit.collider.gameObject.GetComponent<PRVValve>().turn + ")\n Left [Right Click], Right [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (_testMode == TestMode.Screen)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var target = hit.collider.gameObject.GetComponent<PRVValve>();
                        target.TurnValve("right");
                        stateManager.GetComponent<StateManager>().OnChange();

                        // record action
                        // OutputManagerEvents.RecordToOutput(target.id, "Right");
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        var target = hit.collider.gameObject.GetComponent<PRVValve>();
                        target.TurnValve("left");
                        stateManager.GetComponent<StateManager>().OnChange();

                        // record action
                        // OutputManagerEvents.RecordToOutput(target.id, "Left");
                    }
                }
                else
                {
                    InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                    InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                    bool leftPressed = false;
                    bool rightPressed = false;

                    leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftPressed);
                    rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightPressed);

                    bool leftJustPressed = leftPressed && !leftPrevPressed;
                    bool rightJustPressed = rightPressed && !rightPrevPressed;

                    if (leftJustPressed || rightJustPressed)
                    {
                        var target = hit.collider.gameObject.GetComponent<PRVValve>();
                        if (target != null)
                        {
                            target.TurnValve("right");
                            stateManager.GetComponent<StateManager>().OnChange();
                        }
                    }

                    leftPrevPressed = leftPressed;
                    rightPrevPressed = rightPressed;
                }
            }

            if (selection.CompareTag(ComputerTag))
            {
                _interactCaptionActive.text = "Access DeltaV [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (_testMode == TestMode.Screen)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        DeltaV.SetActive(true);
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                }
                else
                {
                    InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                    InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                    bool leftPressed = false;
                    bool rightPressed = false;

                    leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftPressed);
                    rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightPressed);

                    bool leftJustPressed = leftPressed && !leftPrevPressed;
                    bool rightJustPressed = rightPressed && !rightPrevPressed;

                    if (leftJustPressed || rightJustPressed)
                    {
                        var target = hit.collider.gameObject.GetComponent<PRVValve>();
                        if (target != null)
                        {
                            DeltaV.SetActive(true);
                            Cursor.lockState = CursorLockMode.Confined;
                        }
                    }

                    leftPrevPressed = leftPressed;
                    rightPrevPressed = rightPressed;
                }
            }
        }
    }
}