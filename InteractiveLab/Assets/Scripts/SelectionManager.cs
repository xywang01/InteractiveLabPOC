using System;
using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using UnityEngine.UI;

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

    private void Update() {
        if (!_modeIsSet)
        {
            _testMode = ModeManagerEvents.GetCurrentMode();
            SetTestMode(_testMode);
            _modeIsSet = true;
        }
        
        if (Input.GetKeyDown("t")) {
            videoPanel.SetActive(!videoPanel.activeSelf);
        }

        if (_selection != null) {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null) {
                selectionRenderer.material = defaultMaterial;  // Reset material to default
            }
            _selection = null;  // Clear the current selection
            _interactCaptionActive.text = "";  // Clear the interaction caption
            _captionBackgroundActive.enabled = false;  // Disable the caption background
        }
        
        // todo check if I can get raycast from joystick controller
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToSee) && !PauseMenu.paused) 
        {
            
            var selection = hit.transform;
            
            // Debug.Log($"selection tag is {selection.tag}");

            var selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null) {
                selectionRenderer.material = highlightMaterial;
            }
            _selection = selection;

            if (selection.CompareTag(InfoGaugeTag)) {
                _interactCaptionActive.text = hit.collider.gameObject.GetComponent<InfoGauge>().description + hit.collider.gameObject.GetComponent<InfoGauge>().value;
                _captionBackgroundActive.enabled = true;
            }

            if (selection.CompareTag(twoWayValveTag)) {
                var action = "";
                if (hit.collider.gameObject.GetComponent<TwoWayValve>().open) {
                    action = "Close ";
                } else {
                    action = "Open ";
                }
                _interactCaptionActive.text = action + hit.collider.gameObject.GetComponent<TwoWayValve>().id + " [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    var target = hit.collider.gameObject.GetComponent<TwoWayValve>();
                    Debug.Log("Hit " + target.id);
                    target.TurnValve();
                    stateManager.GetComponent<StateManager>().OnChange();
                    
                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, target.open ? "Open" : "Close");
                }
            }

            if (selection.CompareTag(threeWayValveTag)) {
                _interactCaptionActive.text = "Turn " + hit.collider.gameObject.GetComponent<ThreeWayValve>().id + "(" + hit.collider.gameObject.GetComponent<ThreeWayValve>().position + ") [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    var target = hit.collider.gameObject.GetComponent<ThreeWayValve>();
                    Debug.Log("Hit " + target.id);
                    target.TurnValve();
                    stateManager.GetComponent<StateManager>().OnChange();
                    
                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, target.position.ToString());
                }
            }

            if (selection.CompareTag(circleValveTag)) {
                var action = "";
                if (hit.collider.gameObject.GetComponent<CircleValve>().open) {
                    action = "Close ";
                } else {
                    action = "Open ";
                }
                _interactCaptionActive.text = action + hit.collider.gameObject.GetComponent<CircleValve>().id + " [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    var target = hit.collider.gameObject.GetComponent<CircleValve>();
                    Debug.Log("Hit " + target.id);
                    target.TurnValve();
                    stateManager.GetComponent<StateManager>().OnChange();
                    
                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, target.open ? "Open" : "Close");
                }
            }

            if (selection.CompareTag(condensationTrapTag)) {
                _interactCaptionActive.text = "Condensation Trap Liquid Level: " + hit.collider.gameObject.GetComponent<CondensationTrap>().liquidLevel + "%";
                _captionBackgroundActive.enabled = true;
            }

            if (selection.CompareTag(PRVTag)) {
                _interactCaptionActive.text = "Turn " + hit.collider.gameObject.GetComponent<PRVValve>().id + "(current: " + hit.collider.gameObject.GetComponent<PRVValve>().turn + ")\n Left [Right Click], Right [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    var target = hit.collider.gameObject.GetComponent<PRVValve>();
                    target.TurnValve("right");
                    stateManager.GetComponent<StateManager>().OnChange();
                    
                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, "Right");
                }

                if (Input.GetMouseButtonDown(1)) {
                    var target = hit.collider.gameObject.GetComponent<PRVValve>();
                    target.TurnValve("left");
                    stateManager.GetComponent<StateManager>().OnChange();
                    
                    // record action
                    // OutputManagerEvents.RecordToOutput(target.id, "Left");
                }
            }

            if (selection.CompareTag(ComputerTag)) {
                _interactCaptionActive.text = "Access DeltaV [Left Click]";
                _captionBackgroundActive.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    DeltaV.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                }
            }
        }
    }
}
