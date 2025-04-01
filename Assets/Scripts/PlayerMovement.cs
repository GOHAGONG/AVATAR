using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0f;
    public float jumpForce = 0f;

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

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // 이동 적용
        controller.Move(move * moveSpeed * Time.deltaTime);


        // isWalking 판단
        isWalking = move.magnitude > 0.1f;

        // isRunning 판단
        if (Input.GetButtonDown("Run") && isWalking)
        {
            isRunning = true;
        }

        if (Input.GetButtonUp("Run") && isWalking)
        {
            isRunning = false;
        }

        // Animator 연동
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrawling", isCrawling);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            isJumping = true;
            StartCoroutine(WaitAndJump());
        }

        IEnumerator WaitAndJump()
        {
            animator.SetTrigger("Jump Start");
            yield return new WaitForSeconds(0.8f);
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            Debug.Log("Jump action triggered!");
            isJumping = false;
        }

        // Gravity 적용
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Crouch"))
        {
            animator.SetTrigger("Crouch Start");
            isCrouching = true;
        }

        if (Input.GetButtonUp("Crouch") && isCrouching) 
        {
            animator.SetTrigger("Crouch End");
            isCrouching = false;
        }

        // Throw - E key
        if (Input.GetButtonDown("Throw"))
        {
            Debug.Log("Throw action triggered!");
            animator.SetTrigger("Throw Start");
            isThrowing = true;
        }

        if (Input.GetButtonUp("Throw") && isThrowing) 
        {
            animator.SetTrigger("Throw End");
            isThrowing = false;
        }

        // Stomp - Q key
        if (isGrounded && Input.GetButtonDown("Stomp"))
        {
            Debug.Log("Stomp action triggered!");
            animator.SetTrigger("Stomp");
        }

        if (Input.GetButtonDown("Lying Down"))
        {
            Debug.Log("Lying action triggered!");
            animator.SetTrigger("Lying Start");
            isLying = true;
        }

        if (Input.GetButtonUp("Lying Down") && isLying) 
        {
            animator.SetTrigger("Lying End");
            isLying = false;
        }

        if (Input.GetButtonDown("Crawl"))
        {
            Debug.Log("Crawl action triggered!");
            animator.SetTrigger("Crawl Start");
            isCrawling = true;
        }

        if (Input.GetButtonUp("Crawl") && isCrawling) 
        {
            animator.SetTrigger("Crawl End");
            isCrawling = false;
        }
    }
}
