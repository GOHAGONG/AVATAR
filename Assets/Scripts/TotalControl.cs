using UnityEngine;
using System.Collections;
using Valve.VR;

public enum CurrentMethod { Controller, HalfBody, FullBody, Customize }
public enum CustomMethod { Controller, HalfBody, FullBody }

public class TotalControl : MonoBehaviour
{
    [Header("Select Method")]
    [SerializeField]
    public CurrentMethod SelectedMethod = CurrentMethod.Controller;

    [Header("Customize Method")]
    [SerializeField]
    public CustomMethod WalkMethod;
    public CustomMethod CrouchMethod;
    public CustomMethod JumpMethod;
    public CustomMethod CrawlMethod;

    private void OnValidate()
    {
        if (SelectedMethod != CurrentMethod.Customize)
        {
            var method = (CustomMethod)SelectedMethod;
            WalkMethod = method;
            CrouchMethod = method;
            JumpMethod = method;
            CrawlMethod = method;
        }
    }


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    [Header("VR Input")]
    public SteamVR_Action_Vector2 trackpadAxisAction;
    public SteamVR_Action_Boolean jumpAction;
    public SteamVR_Action_Boolean crouchAction;
    public SteamVR_Action_Boolean crawlAction;
    public SteamVR_Input_Sources leftInputSource = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightInputSource = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources? activeHand = null;
    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;
    private Vector3 prevLeftPos;
    private Vector3 prevRightPos;
    private float leftSwingAmount;
    private float rightSwingAmount;

    [Header("Components")]
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isJumping = false;
    public bool isCrawling = false;
    public bool filpLeft = false;
    public float crouchThreshold;
    private bool crouchTriggered = false;
    public float jumpThreshold;
    public float jumpExitThreshold;
    public float crawlThreshold1;
    public float crawlThreshold2;
    public float crawlThreshold3;
    public float crawlThreshold4;
    public float walkThreshold;
    public bool isWalkThreshold;
    public float runThreshold;

    [Header("Movement")]
    public float moveSpeed = 0f;
    public float jumpForce = 0f;
    private float walkTimer = 0f;
    private float runTimer = 0f;
    private float walkHoldTime = 0.9f; // 얼마 동안 유지할지
    private float runHoldTime = 1.0f;
    private float crawlConditionTimer = 0f; // 크롤 조건 유지 시간 추적
    public float crawlHoldTimeRequired = 0.5f; // 0.5초 이상 유지해야 발동
    private bool crawlConditionsHeld = false; // 조건 범위 안에 있는지
    private float crawlStartLeftY;
    private float crawlStartRightY;
    public float crawlExitDelta = 1.3f; // 탈출 기준 변화량 (0.3 이상이면 종료)
    private bool isCrawlingProtected = false; // 크롤 시작 보호 플래그

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        prevLeftPos = leftHandPose.transform.position;
        prevRightPos = rightHandPose.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;


        // For Controller
        Vector2 rightInput = trackpadAxisAction.GetAxis(rightInputSource);
        Vector2 leftInput = trackpadAxisAction.GetAxis(leftInputSource);

        bool leftWalking = leftInput.y > 0.2;
        bool rightWalking = rightInput.y > 0.2;

        if (activeHand == null)
        {
            if (leftWalking)
                activeHand = leftInputSource;
            else if (rightWalking)
                activeHand = rightInputSource;
        }

        if (activeHand == leftInputSource && leftWalking == false)
            activeHand = null;

        if (activeHand == rightInputSource && rightWalking == false)
            activeHand = null;

        Vector2 activeInput = Vector2.zero;

        if (activeHand == leftInputSource)
            activeInput = leftInput;
        else if (activeHand == rightInputSource)
            activeInput = rightInput;


        // For Half Body
        Vector3 leftVel = leftHandPose.GetVelocity();
        Vector3 rightVel = rightHandPose.GetVelocity();

        float leftForward = Vector3.Dot(transform.forward, leftVel);
        float rightForward = Vector3.Dot(transform.forward, rightVel);

        float avgSpeed = (Mathf.Abs(leftForward) + Mathf.Abs(rightForward)) / 2f;

        bool handsAreCrossing = (leftForward > 0.05f && rightForward < -0.05f) ||
                                (leftForward < -0.05f && rightForward > 0.05f);

        float leftY = leftHandPose.transform.position.y;
        float rightY = rightHandPose.transform.position.y;
        float yDiff = Mathf.Abs(leftY - rightY);

        bool idlecheck = !isJumping && !isCrawling && !isCrouching && !isWalking && !isWalking;



        // Walk & Run
        if (WalkMethod == CustomMethod.Controller) {
            if (activeHand != null && !isJumping)
            {
                // 이동벡터 계산
                Vector3 move = transform.right * activeInput.x + transform.forward * activeInput.y;

                // 이동 적용
                controller.Move(move * moveSpeed * Time.deltaTime);

                isWalking = (activeInput.y > 0.2f);
                isRunning = (activeInput.y >= 0.6f);
            }
            else
            {
                isWalking = false;
                isRunning = false;
            }
        }
        else if(WalkMethod == CustomMethod.HalfBody) {
            if (leftY < walkThreshold && rightY < walkThreshold)
                isWalkThreshold = true;
            else isWalkThreshold = false;

            if (!isCrawling && !isCrouching && !isJumping)
            {
                if (isWalkThreshold)
                {
                    // 타이머 업데이트
                    if (handsAreCrossing && avgSpeed > 0.05f)
                    {
                        // 조건 만족했을 때만 타이머 리셋
                        if (avgSpeed >= runThreshold)
                        {
                            runTimer = runHoldTime;
                        }
                        else
                        {
                            walkTimer = walkHoldTime;
                        }
                    }

                    // 타이머 감소
                    if (walkTimer > 0)
                        walkTimer -= Time.deltaTime;
                    if (runTimer > 0)
                        runTimer -= Time.deltaTime;

                    // 상태 판단
                    isRunning = runTimer > 0f;
                    isWalking = walkTimer > 0f;

                    // 디버깅 (선택사항)
                    //Debug.Log($"[Move] Cross: {handsAreCrossing}, Speed: {avgSpeed:F2}, WalkT: {walkTimer:F2}, RunT: {runTimer:F2}");
                }
                else
                {
                    isRunning = false;
                    isWalking = false;
                }
            }
        }
        else { }


        // Crouch
        if (CrouchMethod == CustomMethod.Controller) {
            if (!isCrawling && crouchAction.GetStateDown(rightInputSource))
            {
                animator.SetTrigger("Crouch Start");
                isCrouching = true;
            }

            if ((crouchAction.GetStateUp(rightInputSource) && isCrouching))
            {
                animator.SetTrigger("Crouch End");
                isCrouching = false;
            }

            if (!isCrawling && crouchAction.GetStateDown(leftInputSource))
            {

                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * -1f; // 왼손 입력이면 반전
                transform.localScale = newScale;
                filpLeft = true;

                animator.SetTrigger("Crouch Start");
                isCrouching = true;

            }

            if ((crouchAction.GetStateUp(leftInputSource) && isCrouching))
            {
                animator.SetTrigger("Crouch End");
                isCrouching = false;

                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * 1f; // 왼손 입력이면 반전
                transform.localScale = newScale;
                filpLeft = false;
            }
        }
        else if (CrouchMethod == CustomMethod.HalfBody) {
            if (idlecheck && yDiff >= crouchThreshold && !crouchTriggered)
            {
                animator.SetTrigger("Crouch Start");
                isCrouching = true;
                crouchTriggered = true;
                Debug.Log("Crouch Start");
            }

            if (isCrouching && yDiff < crouchThreshold * 0.5f && crouchTriggered)
            {
                animator.SetTrigger("Crouch End");
                isCrouching = false;
                crouchTriggered = false;
                Debug.Log("Crouch End");
            }

            Debug.Log("leftY: " + leftY);
        }
        else { }
        

        // Jump
        if (JumpMethod == CustomMethod.Controller) {
            bool leftPressed = jumpAction.GetStateDown(leftInputSource);
            bool rightPressed = jumpAction.GetStateDown(rightInputSource);



            if (!isJumping && rightPressed)
            {
                isJumping = true;
                animator.SetTrigger("Jump Prepare");
                Debug.Log("Jump preparation started");
                if (activeHand == rightInputSource)
                    activeHand = leftInputSource;
            }

            if (isJumping && activeInput.y > 0.2f)
            {
                animator.SetTrigger("Jump Execute");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                Debug.Log("Jump executed!");
                isJumping = false;
            }

            if ((jumpAction.GetStateUp(leftInputSource) && isJumping) ||
                (jumpAction.GetStateUp(rightInputSource) && isJumping))
            {
                animator.SetTrigger("Jump Cancel");
                isJumping = false;
            }
        }
        else if (JumpMethod == CustomMethod.HalfBody) {
            if (idlecheck && leftY >= jumpThreshold && rightY >= jumpThreshold)
            {
                isJumping = true;
                animator.SetTrigger("Jump Prepare");
            }
            if (isJumping && leftY < jumpExitThreshold && rightY < jumpExitThreshold)
            {
                animator.SetTrigger("Jump Execute");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                Debug.Log("Jump executed!");
                isJumping = false;
            }
        }
        else { }


        // Crawl
        if (CrawlMethod == CustomMethod.Controller) {
            if (!isCrouching && crawlAction.GetStateDown(leftInputSource))
            {
                Debug.Log("Crawl Left Triggered");
                animator.SetTrigger("Crawl Start");
                isCrawling = true;

                if (activeHand == leftInputSource)
                    activeHand = rightInputSource;
            }
            if (!isCrouching && crawlAction.GetStateDown(rightInputSource))
            {
                Debug.Log("Crawl Right Triggered");
                animator.SetTrigger("Crawl Start");
                isCrawling = true;

                if (activeHand == rightInputSource)
                    activeHand = leftInputSource;
            }

            if (WalkMethod != CustomMethod.Controller)
            {
                if (activeHand != null && !isJumping && isCrawling)
                {
                    // 이동벡터 계산
                    Vector3 move = transform.right * activeInput.x + transform.forward * activeInput.y;

                    // 이동 적용
                    controller.Move(move * moveSpeed * Time.deltaTime);

                    isWalking = (activeInput.y > 0.2f);
                    isRunning = (activeInput.y >= 0.6f);
                }
                else
                {
                    isWalking = false;
                    isRunning = false;
                }
            }

            if ((crawlAction.GetStateUp(leftInputSource) && isCrawling) ||
                (crawlAction.GetStateUp(rightInputSource) && isCrawling))
            {
                animator.SetTrigger("Crawl End");
                isCrawling = false;
            }
        }
        else if (CrawlMethod == CustomMethod.HalfBody) {
            bool inCrawlRange = leftY > crawlThreshold1 && leftY < crawlThreshold2 &&
                                rightY > crawlThreshold1 && rightY < crawlThreshold2;
            if (idlecheck && inCrawlRange)
            {
                crawlConditionTimer += Time.deltaTime; // 조건 만족 중 → 타이머 증가

                if (crawlConditionTimer >= crawlHoldTimeRequired && !isCrawling)
                {
                    //Debug.Log("Crawl Triggered after Hold");
                    animator.SetTrigger("Crawl Start");
                    isCrawling = true;

                    // 보호 타이머 시작
                    // 애니메이션 재생동안 Crawl Exit Delta 계산하지 않도록
                    StartCoroutine(CrawlProtectionDelay(2.0f));

                    crawlConditionTimer = 0f; // 초기화
                }
            }
            else
            {
                crawlConditionTimer = 0f; // 범위 벗어나면 타이머 리셋
            }

            if (isCrawling)
            {
                // 타이머 업데이트
                if (handsAreCrossing && avgSpeed > 0.05f)
                {
                    // // 조건 만족했을 때만 타이머 리셋
                    // if (avgSpeed >= 0.4f)
                    // {
                        // runTimer = runHoldTime;
                    // }
                    // else
                    // {
                        walkTimer = walkHoldTime;
                    // }
                }

                // 타이머 감소
                if(walkTimer > 0)
                    walkTimer -= Time.deltaTime;
                // if(runTimer > 0)
                    // runTimer -= Time.deltaTime;

                // 상태 판단
                // isRunning = runTimer > 0f;
                isWalking = (isCrawling && walkTimer > 0f);
            }

            // if (isCrawling && ((leftY < crawlThreshold3 && rightY < crawlThreshold3) || (leftY > crawlThreshold4 && rightY > crawlThreshold4)) )
            // {
            //     animator.SetTrigger("Crawl End");
            //     isCrawling = false;
            // }

            if (isCrawling && !isCrawlingProtected)
            {
                float leftDelta = Mathf.Abs(leftY - crawlStartLeftY);
                float rightDelta = Mathf.Abs(rightY - crawlStartRightY);

                Debug.Log("left Delta: " + leftDelta);

                if (leftDelta > crawlExitDelta && rightDelta > crawlExitDelta)
                {
                    //Debug.Log("Crawl End triggered by movement delta");
                    animator.SetTrigger("Crawl End");
                    isCrawling = false;
                }
            }
        }
        else { }

        // Animator 연동
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrawling", isCrawling);
        }
    }
    private IEnumerator CrawlProtectionDelay(float delay)
    {
        isCrawlingProtected = true;
        yield return new WaitForSeconds(delay);
        isCrawlingProtected = false;

        // 엎드렸을 때 손 위치 초기화
        crawlStartLeftY = leftHandPose.transform.position.y; // leftY
        crawlStartRightY = rightHandPose.transform.position.y; // rightY
        Debug.Log("Crawl Start");
    }
}