using UnityEngine;
using System.Collections;

public class KeyboardCtrl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 3f;

    [Header("Components")]
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isThrowing = false;
    public bool isLying = false;
    public bool isJumping = false;
    public bool isCrawling = false;

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

        // Movement Input (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Walking check
        isWalking = move.magnitude > 0.1f;
        if (move.z < 0)
            isWalking = false;

        // Animator sync
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrawling", isCrawling);
        }

        // Jump (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            isJumping = true;
            animator.SetTrigger("Jump Prepare");
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded && isJumping)
        {
            StartCoroutine(WaitAndJump());
        }

        // Gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Crouch (Left Ctrl)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("Crouch Start");
            isCrouching = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
        {
            animator.SetTrigger("Crouch End");
            isCrouching = false;
        }

        // Crawl (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Crawl action triggered!");
            animator.SetTrigger("Crawl Start");
            isCrawling = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isCrawling)
        {
            animator.SetTrigger("Crawl End");
            isCrawling = false;
        }

        // (Optional) Throw action placeholder (if needed)
        if (Input.GetKeyUp(KeyCode.T) && isThrowing)
        {
            animator.SetTrigger("Throw End");
            isThrowing = false;
        }
    }

    IEnumerator WaitAndJump()
    {
        animator.SetTrigger("Jump Execute");
        yield return new WaitForSeconds(0.3f);
        velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        isJumping = false;
    }
}
