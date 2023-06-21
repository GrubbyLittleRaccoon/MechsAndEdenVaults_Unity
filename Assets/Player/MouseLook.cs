using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 300f;

    public Transform playerBody; // Note this has to be set in the inspector

    public float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to centre
    }

    // Update is called once per frame
    void Update()
    {
        // Unity -> Edit -> ProjectSettings -> Input for more input data
        // deltaTime = time since last update

        // Grab inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; 
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Quaternions do rotation
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
