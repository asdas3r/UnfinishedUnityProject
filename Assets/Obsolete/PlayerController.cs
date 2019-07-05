using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float jumpHeight = 1.5f;
    [SerializeField]
    private float fallMultiplier = 2.5f;

    [Space]
    private float walkSpeed = 6.0f;
    [SerializeField]
    private float runSpeed = 12.0f;
    [SerializeField]
    private float runBuildUp = 0.1f;
    [SerializeField]
    private KeyCode runKey;

    float distanceToGround;
    Vector3 moveDirection;
    Rigidbody rb;
    bool isJumping;
    bool isRunning;
    float timeOffTheGround;
    float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.1f;
        timeOffTheGround = 0f;
        speed = walkSpeed;
        isJumping = false;
        isRunning = false;
    }

    void Update()
    {
        SprintInput();
        MoveInputs();
        JumpInput();

        if (PauseMenu.isPaused)
        {
            moveDirection = Vector3.zero;
            isJumping = false;
            return;
        }
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void MoveInputs()
    {
        float xMovInput = Input.GetAxis("Horizontal");
        float zMovInput = Input.GetAxis("Vertical");
        moveDirection = Vector3.ClampMagnitude(transform.right * xMovInput + transform.forward * zMovInput, 1.0f);

        //Debug.Log(IsGrounded());
        //Debug.DrawRay(transform.position, new Vector3(0, -distanceToGround, 0), Color.red);
    }

    void Move()
    {
        rb.MovePosition(rb.transform.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void JumpInput()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;
        }
    }

    void Jump()
    {
        if (isJumping)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics.gravity.y * jumpHeight), ForceMode.VelocityChange);
            isJumping = false;
        }
        
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
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

    bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, distanceToGround))
        {
            return true;
        }

        return false;
    }
}
