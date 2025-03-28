using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 2f;

    [Header("Components")]
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching;
    public bool isWalking;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // 이동 적용
        controller.Move(move * moveSpeed * Time.deltaTime);

        // isWalking 판단
        isWalking = move.magnitude > 0.1f;

        // Animator 연동
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            Debug.Log("Jump action triggered!");
            animator.SetTrigger("Jump");
        }

        // Gravity 적용
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Crouch - Left Ctrl
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }

        // Throw - E key
        if (Input.GetButtonDown("Throw"))
        {
            Debug.Log("Throw action triggered!");
            animator.SetTrigger("Throw");
        }

        // Stomp - Q key
        if (isGrounded && Input.GetButtonDown("Stomp"))
        {
            Debug.Log("Stomp action triggered!");
            animator.SetTrigger("Stomp");
        }
    }
}
