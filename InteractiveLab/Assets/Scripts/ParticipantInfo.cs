using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticipantInfo : MonoBehaviour
{
    private TMP_Dropdown sexInput;

    public TestMode testMode;

    public TMP_InputField nonVRAgeInput;
    public TMP_InputField nonVRIdInput;
    public TMP_Dropdown nonVRSexInput;

    public TMP_Dropdown VRAgeInput;
    public TMP_Dropdown VRIdInput;
    public TMP_Dropdown VRSexInput;

    [SerializeField] private MouseLook mouseLook;
    public GameObject nonVRInfoUI;
    public GameObject VRInfoUI;
    private GameObject infoUI;
    public Recording.OutputManager outputManager;
    public Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        if (testMode == TestMode.Screen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            sexInput = nonVRSexInput;
            infoUI = nonVRInfoUI;
            nonVRInfoUI.SetActive(true);
            VRInfoUI.SetActive(false);
        }
        else
        {
            sexInput = VRSexInput;
            infoUI = VRInfoUI;
            nonVRInfoUI.SetActive(false);
            VRInfoUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Submit()
    {
        string age;
        string id;
        string sex = sexInput.options[sexInput.value].text;

        if (testMode == TestMode.Screen)
        {
            age = nonVRAgeInput.text;
            id = nonVRIdInput.text;
        }
        else
        {
            age = VRAgeInput.options[VRAgeInput.value].text;
            id = VRIdInput.options[VRIdInput.value].text;
        }

        infoUI.SetActive(false);
        outputManager.Setup(age, id, sex);
        mouseLook.ActivateMouseLook();
        movement.ActivateMovement();
        Time.timeScale = 1f;
        Cursor.visible = false;
    }
}
