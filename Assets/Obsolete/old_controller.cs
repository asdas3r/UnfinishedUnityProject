using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController_old : MonoBehaviour
{
    [SerializeField]
    private float jumpMultiplier = 1.5f;
    [SerializeField]
    private AnimationCurve jumpCurve;

    [Space]
    [SerializeField]
    private float slopeForce;
    [SerializeField]
    private float slopeForceRayLength;

    [Space]
    [SerializeField]
    private float walkSpeed = 6.0f;
    [SerializeField]
    private float runSpeed = 12.0f;
    [SerializeField]
    private float runBuildUp = 0.1f;
    [SerializeField]
    private KeyCode runKey;

    [Space]
    [SerializeField]
    private float inputAxisSensitivity = 3.0f;
    [SerializeField]
    private float inputAxisDeadZone = 0.002f;

    private CharacterController characterController;
    Vector3 moveDirection;
    bool isJumping;
    bool isRunning;
    float speed;
    float horizontalInput;
    float verticalInput;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        isJumping = false;
        isRunning = false;
    }

    void Update()
    {
        if (PauseMenu.isPaused)
        {
            MoveInput(0f, 0f);
        }
        else
        {
            MoveInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SprintInput();
            JumpInput();
        }

        Move();
    }

    void MoveInput(float horizontal, float vertical)
    {
        GetAxisSmoothing(ref horizontalInput, horizontal);
        GetAxisSmoothing(ref verticalInput, vertical); 
    }

    void GetAxisSmoothing(ref float axisInputVar, float axisValue)
    {
        axisInputVar = Mathf.MoveTowards(axisInputVar, axisValue, inputAxisSensitivity * Time.deltaTime);
        axisInputVar = (Mathf.Abs(axisInputVar) < inputAxisDeadZone) ? 0f : axisInputVar;
    }

    void Move()
    {
        moveDirection = Vector3.ClampMagnitude(transform.right * horizontalInput + transform.forward * verticalInput, 1.0f) * speed;

        if ((horizontalInput != 0 || verticalInput != 0) && OnSlope())
        {
            characterController.Move(Vector3.down * (characterController.height / 2) * slopeForce * Time.deltaTime);
        }

        characterController.SimpleMove(moveDirection);
    }

    void JumpInput()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        float jumpTime = 0.0f;
        //characterController.slopeLimit = 90f;

        do
        {
            float jumpForce = jumpCurve.Evaluate(jumpTime);
            characterController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            jumpTime += Time.deltaTime;
            yield return null;
        } while (!characterController.isGrounded && characterController.collisionFlags != CollisionFlags.Above);

        isJumping = false;
        //characterController.slopeLimit = 45f;
    }

    void SprintInput()
    {
        if (Input.GetKey(runKey) && Input.GetAxisRaw("Vertical") == 1.0f)
        {
            if (!isRunning)
            {
                StartCoroutine(Sprint());
            }
        }

        if (!isRunning)
        {
            speed = Mathf.Lerp(speed, walkSpeed, runBuildUp);
        }
    }

    IEnumerator Sprint()
    {
        isRunning = true;

        do
        {
            speed = Mathf.Lerp(speed, runSpeed, runBuildUp);
            yield return null;
        } while (Input.GetAxisRaw("Vertical") == 1.0f);

        isRunning = false;
    }

    bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }

        return false;
    }
}
