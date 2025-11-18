using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public CharacterController controller;
    public GameObject HX;
    public GameObject HXPivot;
    private bool freeControl = true;
    public float speed = 500f;
    public float rotateSpeed = 50f;
    Vector3 playerPosition;
    Vector3 HXPosition;

    void Start() {
        playerPosition = transform.position;
        HXPosition = HX.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left shift")) {
            freeControl = !freeControl;
            if (freeControl) {
                HX.transform.position = HXPosition;
                HX.transform.rotation = Quaternion.identity;
            } else {
                transform.position = playerPosition;
            }
        }

        if (freeControl) {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (controller.isGrounded == false) {
                move += Physics.gravity;
            }

            controller.Move(move * speed * Time.deltaTime);
        } else {
            if (Input.GetKey("a")) {
                HX.transform.RotateAround(HXPivot.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            }
            
            if (Input.GetKey("d")) {
                HX.transform.RotateAround(HXPivot.transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
            }
        }
    }
}
