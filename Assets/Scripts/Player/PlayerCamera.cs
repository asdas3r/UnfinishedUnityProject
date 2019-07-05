using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 100.0f;
    [SerializeField] float verticalMaxAngle = 90f;
    [SerializeField] float verticalMinAngle = -90f;

    public Camera playerCamera;
    //Rigidbody rb;
    float rotationX;

    void Start()
    {
        rotationX = 0f;
    }

    void Update()
    {
        if (PauseMenu.isPaused)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        CameraRotation();
    }

    void CameraRotation()
    {
        float bodyRotation = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float cameraRotation = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationX += cameraRotation;
        rotationX = Mathf.Clamp(rotationX, verticalMinAngle, verticalMaxAngle);

        transform.Rotate(Vector3.up * bodyRotation);
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * bodyRotation));
        playerCamera.transform.localEulerAngles = Vector3.left * rotationX;
    }
}