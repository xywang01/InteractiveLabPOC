using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticipantInfo : MonoBehaviour
{
    [SerializeField] private TMP_InputField ageInput;
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_Dropdown sexInput;

    [SerializeField] private MouseLook mouseLook;
    public GameObject infoUI;
    public Recording.OutputManager outputManager;
    public Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Submit()
    {
        string age = ageInput.text;
        string id = idInput.text;
        string sex = sexInput.options[sexInput.value].text;

        infoUI.SetActive(false);
        outputManager.Setup(age, id, sex);
        mouseLook.ActivateMouseLook();
        movement.ActivateMovement();
    }
}
