using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float jumpMultiplier = 1.5f;
    [SerializeField] float jumpForce = 8.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float fallThreshold = 10.0f;

    [Space]
    [SerializeField] float slideLimit = 60;
    [SerializeField] float slopeSlideSpeed = 9.0f;
    [SerializeField] float slopeRayLengthFactor = 1.5f;
    [SerializeField] float antiBumpForce = 0.75f;

    [Space]
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float runSpeed = 12.0f;
    [SerializeField] float runBuildUpTime = 0.5f;
    [SerializeField] KeyCode runKey;

    [Space]
    [SerializeField] float inputAxisSensitivity = 10.0f;
    [SerializeField] float inputAxisGravity = 3.0f;
    [SerializeField] float inputAxisDeadZone = 0.002f;
    [SerializeField] float airMovementSensitivity = 2.5f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    bool isPlayerInControl;
    bool isGrounded = false;
    bool isFalling;
    bool isJumping;
    bool isRunning;
    float speed;
    float horizontalInput;
    float verticalInput;
    RaycastHit rayHit;
    float rayDistance;
    float fallStartHeight;
    Vector3 contactColliderPoint;
    float horizontalMoveInputSensitivity;
    float verticalMoveInputSensitivity;
    Vector3 forward;
    Vector3 right;
    float gravityValue;
    IEnumerator coroutine;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        isJumping = false;
        isRunning = false;
        forward = Vector3.zero;
        right = Vector3.zero;
        gravityValue = 0f;

        horizontalMoveInputSensitivity = inputAxisSensitivity;
        verticalMoveInputSensitivity = inputAxisSensitivity; 
        speed = walkSpeed;
        rayDistance = characterController.height * 0.5f + characterController.skinWidth;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (PauseMenu.isPaused)
        {
            MoveInput(0f, 0f);
            isPlayerInControl = false;
        }
        else
        {
            MoveInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            isPlayerInControl = true;
        }
        
        FallingControl();
        MovementControl();

        if (isGrounded)
        {
            SprintInput();
            JumpInput();
        }

        Move();
    }

    void MoveInput(float horizontal, float vertical)
    {
        if (!isGrounded)
        {
            if (horizontal == 0)
            {
                horizontalMoveInputSensitivity = 0;
            }
            else
            {
                horizontalMoveInputSensitivity = airMovementSensitivity;
            }

            if (vertical == 0)
            {
                verticalMoveInputSensitivity = 0;
            }
            else
            {
                verticalMoveInputSensitivity = airMovementSensitivity;
            }
        }
        else
        {
            if (horizontal == 0)
            {
                horizontalMoveInputSensitivity = inputAxisGravity;
            }
            else
            {
                horizontalMoveInputSensitivity = inputAxisSensitivity;
            }

            if (vertical == 0)
            {
                verticalMoveInputSensitivity = inputAxisGravity;
            }
            else
            {
                verticalMoveInputSensitivity = inputAxisSensitivity;
            }
        }

        GetAxisSmoothing(ref horizontalInput, horizontal, horizontalMoveInputSensitivity);
        GetAxisSmoothing(ref verticalInput, vertical, verticalMoveInputSensitivity);
    }

    void GetAxisSmoothing(ref float axisInputVar, float axisValue, float moveInputSensitivity)
    {
        if (((axisInputVar < 0 && axisValue > 0) || (axisInputVar > 0 && axisValue < 0)) && isGrounded)
        {
            axisInputVar = 0.0f;
        }
        else
        {
            axisInputVar = Mathf.MoveTowards(axisInputVar, axisValue, moveInputSensitivity * Time.deltaTime);
        }

        axisInputVar = (Mathf.Abs(axisInputVar) < inputAxisDeadZone) ? 0f : axisInputVar;
    }

    void JumpInput()
    {
        if (isPlayerInControl && Input.GetButtonDown("Jump") && !isJumping)
        {
            //moveDirection.y = jumpForce;
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        isJumping = true;
        float jumpTime = 0.0f;
        float _jumpForce = 0.0f;
        float maxHeight = 0.0f;
        float upTime = 0.0f;
        float slopeLimitCC = 0f;
        float thisMoveValue = 0.0f;
        float prevMoveValue = 0.0f;
        bool sideCollision = false;
        moveDirection.y = 0;

        slopeLimitCC = characterController.slopeLimit;
        characterController.slopeLimit = 90f;

        do
        {
            if (!sideCollision && characterController.collisionFlags == CollisionFlags.Sides)
            {
                Debug.Log("SideCollision");
                sideCollision = true;
            }

            if (sideCollision)
            {
                speed = isRunning ? runSpeed / 3 : walkSpeed / 3;
            }

            _jumpForce = jumpCurve.Evaluate(jumpTime);
            thisMoveValue = Vector3.up.y * _jumpForce * jumpMultiplier;
            moveDirection.y += thisMoveValue;
            moveDirection.y -= prevMoveValue;
            prevMoveValue = thisMoveValue;
            jumpTime += Time.deltaTime;
            if (transform.position.y > maxHeight)
            {
                maxHeight = transform.position.y;
                upTime = jumpTime;
            }
            yield return null;
        } while ((!characterController.isGrounded) && (characterController.collisionFlags != CollisionFlags.Above));

        moveDirection.y -= prevMoveValue;
        Debug.Log("Last jumpForce value: " + _jumpForce);
        Debug.Log("Peak time: " + upTime);
        Debug.Log("Uptime: " + jumpTime);
        isJumping = false;

        if (sideCollision)
        {
            speed = walkSpeed;
            if (isRunning)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = SpeedLerp(speed, walkSpeed, runSpeed, runBuildUpTime);
                StartCoroutine(coroutine);
            }
        }

        characterController.slopeLimit = slopeLimitCC;
    }

    void SprintInput()
    {
        if (isPlayerInControl && Input.GetKey(runKey) && Input.GetAxisRaw("Vertical") == 1.0f)
        {
            if (!isRunning)
            {
                StartCoroutine(Sprint());
            }
        }
        
        if (!isRunning)
        {

        }
    }

    IEnumerator Sprint()
    {
        isRunning = true;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = SpeedLerp(speed, walkSpeed, runSpeed, runBuildUpTime);
        StartCoroutine(coroutine);

        do
        {

            yield return null;
        } while (Input.GetAxisRaw("Vertical") == 1.0f);

        isRunning = false;

        StopCoroutine(coroutine);
        coroutine = SpeedLerp(speed, runSpeed, walkSpeed, runBuildUpTime / 2);
        StartCoroutine(coroutine);
    }

    IEnumerator SpeedLerp(float speedNow, float speed1, float speed2, float buildUpTime)
    {
        float t = 0.0f;
        float distance = Mathf.Abs(speed2 - speed1);
        float covered = Mathf.Abs(speed2 - speedNow);
        float timeLeft = covered / distance * buildUpTime;

        while ((speed != speed2))
        {
            speed = Mathf.Lerp(speedNow, speed2, t);
            t += 1 / timeLeft * Time.deltaTime;
            //Debug.Log("Speed: " + speed + " ; speed1: " + speed1 + " ; speed2: " + speed2);
            yield return null;
        }
    }

    void FallingControl()
    {
        if (isGrounded)
        {
            if (isFalling)
            {
                isFalling = false;
                
                if (transform.position.y < fallStartHeight - fallThreshold)
                {
                    //F A L L D A M A G E
                }
            }
        }
        else
        {
            if (!isFalling)
            {
                isFalling = true;
                fallStartHeight = transform.position.y;
            }
            else
            {
                if (transform.position.y > fallStartHeight)
                {
                    fallStartHeight = transform.position.y;
                }
            }
        }
    }

    bool IsSliding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance))
        {
            if ((Vector3.Angle(rayHit.normal, Vector3.up) > slideLimit) && (!Mathf.Approximately(Vector3.Angle(rayHit.normal, Vector3.up), slideLimit)))
            {
                return true;
            }
        }
        else
        {
            if (Physics.Raycast(contactColliderPoint + Vector3.up, -Vector3.up, out rayHit))
            {
                if ((Vector3.Angle(rayHit.normal, Vector3.up) > slideLimit) && (!Mathf.Approximately(Vector3.Angle(rayHit.normal, Vector3.up), slideLimit)))
                {
                    return true;
                }
            }
        }

        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.red);
        Debug.DrawRay(contactColliderPoint + Vector3.up, -Vector3.up, Color.green);
        return false;
    }

    void MovementControl()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            right = transform.right;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            forward = transform.forward;
        }

        if (isGrounded)
        {
            if (IsSliding())
            {
                Vector3 rayHitNormal = rayHit.normal;
                moveDirection = new Vector3(rayHitNormal.x, -rayHitNormal.y, rayHitNormal.z);
                Vector3.OrthoNormalize(ref rayHitNormal, ref moveDirection);
                moveDirection *= slopeSlideSpeed;
                isPlayerInControl = false;
            }
            else
            {
                moveDirection = Vector3.ClampMagnitude(right * horizontalInput + forward * verticalInput, 1.0f);
                moveDirection.y = -antiBumpForce;
                moveDirection *= speed;
            }
        }
        else
        {
            float gravValue = moveDirection.y;
            moveDirection = Vector3.ClampMagnitude(right * horizontalInput + forward * verticalInput, 1.0f) * speed;
            moveDirection.y = gravValue;
        }
    }

    void Move()
    {

        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            //Debug.Log(moveDirection.y);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactColliderPoint = hit.point;
    }
}
