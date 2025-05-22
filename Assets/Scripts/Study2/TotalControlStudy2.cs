using UnityEngine;
using System.Collections;
using Valve.VR;

public enum Current2Method { Controller, HalfBody, FullBody, Customize }
public enum Custom2Method { Controller, HalfBody, FullBody }

public class TotalControlStudy2 : MonoBehaviour
{
    [Header("Select Method")]
    [SerializeField]
    public Current2Method SelectedMethod = Current2Method.Controller;

    [Header("Customize Method")]
    [SerializeField]
    public Custom2Method WalkMethod;
    public Custom2Method CrouchMethod;
    public Custom2Method JumpMethod;
    public Custom2Method CrawlMethod;

    private void OnValidate()
    {
        if (SelectedMethod != Current2Method.Customize)
        {
            var method = (Custom2Method)SelectedMethod;
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

    [Header("Ceiling Check (for Crawl)")]
    public Transform headCheck;
    public float checkRadius = 1.2f;
    public LayerMask ceilingMask;
    public bool canStand = true;

    [Header("Wall Check")]
    private CapsuleCollider capsuleCollider;
    private float originalCapsuleHeight;
    private Vector3 originalCapsuleCenter;

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
    public Transform headPos;
    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;
    public SteamVR_Behaviour_Pose leftFootPose;
    public SteamVR_Behaviour_Pose rightFootPose;
    private Vector3 prevLeftHandPos;
    private Vector3 prevRightHandPos;
    private Vector3 prevLeftFootPos;
    private Vector3 prevRightFootPos;
    private float leftSwingAmount;
    private float rightSwingAmount;

    [Header("For crawlcolider control")]
    public float originalHeight;
    private Vector3 originalCenter;
    public float crawlHeight = 1.0f;
    public Vector3 crawlCenter = new Vector3(0f, 0.5f, 0f);


    [Header("Components")]
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isJumpPrepared = false;
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

    [Header("Scripts 참조")]
    public CameraRigAlignerStudy2 cameraRigAlignerStudy2;
    public Test2Manager test2Manager;

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
    public bool isJumpingProtected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        prevLeftHandPos = leftHandPose.transform.position;
        prevRightHandPos = rightHandPose.transform.position;

        // for decrease collider
        originalHeight = controller.height;
        originalCenter = controller.center;

        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            originalCapsuleHeight = capsuleCollider.height;
            originalCapsuleCenter = capsuleCollider.center;
        }       
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
        Vector3 leftHandVel = leftHandPose.GetVelocity();
        Vector3 rightHandVel = rightHandPose.GetVelocity();
        Vector3 leftFootVel = leftFootPose.GetVelocity();
        Vector3 rightFootVel = rightFootPose.GetVelocity();

        float leftHandForward = Vector3.Dot(transform.forward, leftHandVel);
        float rightHandForward = Vector3.Dot(transform.forward, rightHandVel);
        float leftFootForward = Vector3.Dot(transform.forward, leftFootVel);
        float rightFootForward = Vector3.Dot(transform.forward, rightFootVel);

        float avgHandSpeed = (Mathf.Abs(leftHandForward) + Mathf.Abs(rightHandForward)) / 2f;
        float avgFootSpeed = (Mathf.Abs(leftFootForward) + Mathf.Abs(rightFootForward)) / 2f;

        bool handsAreCrossing = (leftHandForward > 0.05f && rightHandForward < -0.05f) ||
                                (leftHandForward < -0.05f && rightHandForward > 0.05f);
        bool feetAreCrossing = (leftFootForward > 0.05f && rightFootForward < -0.05f) ||
                                (leftFootForward < -0.05f && rightFootForward > 0.05f);

        float headY = headPos.transform.position.y;
        float leftHandY = leftHandPose.transform.position.y;
        float rightHandY = rightHandPose.transform.position.y;
        float leftFootY = leftFootPose.transform.position.y;
        float rightFootY = rightFootPose.transform.position.y;
        float yDiff = Mathf.Abs(leftHandY - rightHandY);

        bool idlecheck = !isJumping && !isCrawling && !isCrouching && !isWalking && !isWalking;



        // Walk & Run
        if (WalkMethod == Custom2Method.Controller)
        {
            UpdateControlTypeUI("Controller");
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
        else if (WalkMethod == Custom2Method.HalfBody)
        {
            UpdateControlTypeUI("Half Body");
            if (leftHandY < walkThreshold && rightHandY < walkThreshold)
                isWalkThreshold = true;
            else isWalkThreshold = false;

            if (!isCrawling && !isCrouching && !isJumping)
            {
                if (isWalkThreshold)
                {
                    // 타이머 업데이트
                    if (handsAreCrossing && avgHandSpeed > 0.1f)
                    {
                        // 조건 만족했을 때만 타이머 리셋
                        if (avgHandSpeed >= runThreshold)
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
        else
        {
            UpdateControlTypeUI("Full Body");
            if (leftHandY < walkThreshold && rightHandY < walkThreshold)
                isWalkThreshold = true;
            else isWalkThreshold = false;

            if (!isCrawling && !isCrouching && !isJumping)
            {
                if (isWalkThreshold)
                {
                    // 타이머 업데이트
                    if (handsAreCrossing && avgHandSpeed > 0.05f && feetAreCrossing && avgFootSpeed > 0.05f)
                    {
                        // 조건 만족했을 때만 타이머 리셋
                        if (avgFootSpeed >= 0.9f)
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
                }
                else
                {
                    isRunning = false;
                    isWalking = false;
                }
            }
        }


        // Crouch
        if (CrouchMethod == Custom2Method.Controller)
        {
            UpdateControlTypeUI("Controller");
            if (!isCrawling && crouchAction.GetStateDown(rightInputSource))
            {
                DecreaseCollider();
                animator.SetTrigger("Crouch Start");
                isCrouching = true;
            }

            if ((crouchAction.GetStateUp(rightInputSource) && isCrouching))
            {
                DefaultCollider();
                animator.SetTrigger("Crouch End");
                isCrouching = false;
            }

            if (!isCrawling && crouchAction.GetStateDown(leftInputSource))
            {

                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * -1f; // 왼손 입력이면 반전
                transform.localScale = newScale;
                filpLeft = true;

                DecreaseCollider();
                animator.SetTrigger("Crouch Start");
                isCrouching = true;

            }

            if ((crouchAction.GetStateUp(leftInputSource) && isCrouching))
            {
                DefaultCollider();
                animator.SetTrigger("Crouch End");
                isCrouching = false;

                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * 1f; // 왼손 입력이면 반전
                transform.localScale = newScale;
                filpLeft = false;
            }
        }
        else if (CrouchMethod == Custom2Method.HalfBody)
        {
            UpdateControlTypeUI("Half Body");
            if (idlecheck && yDiff >= crouchThreshold && !crouchTriggered)
            {
                DecreaseCollider();
                animator.SetTrigger("Crouch Start");
                isCrouching = true;
                crouchTriggered = true;
                Debug.Log("Crouch Start");
            }

            if (isCrouching && yDiff < crouchThreshold * 0.5f && crouchTriggered)
            {
                DefaultCollider();
                animator.SetTrigger("Crouch End");
                isCrouching = false;
                crouchTriggered = false;
                Debug.Log("Crouch End");
            }

            // Debug.Log("leftY: " + leftHandY);
        }
        else
        {
            UpdateControlTypeUI("Full Body");
            if (idlecheck && /* (headY - leftFootY) < 1.2f && (headY - leftFootY) > 0.8f && */
                Mathf.Abs(rightHandY - leftFootY) < 0.1f && Mathf.Abs(leftHandY - leftFootY) > 0.3f && !crouchTriggered)
            {
                DecreaseCollider();
                animator.SetTrigger("Crouch Start");
                // cameraRigAligner.rootOffset = new Vector3(0.0f, 0.0f, 0.0f);
                isCrouching = true;
                crouchTriggered = true;
                Debug.Log("Crouch Start");
            }

            if (isCrouching && /* ((headY - leftFootY) > 1.3f || (headY - leftFootY) < 0.7f) && */
                (rightHandY - leftFootY) > crouchThreshold && crouchTriggered)
            {
                DefaultCollider();
                animator.SetTrigger("Crouch End");
                // cameraRigAligner.rootOffset = new Vector3(0.0f, 1.0f, 0.0f);
                isCrouching = false;
                crouchTriggered = false;
                Debug.Log("Crouch End");
            }
        }


        // Jump
        if (JumpMethod == Custom2Method.Controller)
        {
            UpdateControlTypeUI("Controller");
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
        else if (JumpMethod == Custom2Method.HalfBody)
        {
            UpdateControlTypeUI("Half Body");
            if (idlecheck && leftHandY >= jumpThreshold && rightHandY >= jumpThreshold)
            {
                isJumping = true;
                animator.SetTrigger("Jump Prepare");
            }
            if (isJumping && leftHandY < jumpExitThreshold && rightHandY < jumpExitThreshold)
            {
                animator.SetTrigger("Jump Execute");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                Debug.Log("Jump executed!");
                isJumping = false;
            }
        }
        else
        {
            UpdateControlTypeUI("Full Body");
            if (!isJumpPrepared && idlecheck && leftHandY >= jumpThreshold && rightHandY >= jumpThreshold)
            {
                animator.SetTrigger("Jump Prepare");
                isJumpPrepared = true;
            }

            if (isJumpPrepared && leftFootY > 0.15f && rightFootY > 0.15f)
            {
                isJumpPrepared = false;
                animator.SetTrigger("Jump Execute");
                isJumping = true;
                Debug.Log("Jump executed!");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                StartCoroutine(JumpProtectionDelay(1.2f));
            }

            if (!isJumping && isJumpPrepared && !isJumpingProtected && leftHandY < jumpExitThreshold && rightHandY < jumpExitThreshold)
            {
                isJumpPrepared = false;
                animator.SetTrigger("Jump Cancel");
                Debug.Log("Jump canceled!");
            }
        }


        // Crawl

        //Check Ceiling
        //when Crawling, collider of banana man shoud shrink to crwaling size of man 

        // check can stand when crawling
        canStand = !Physics.CheckSphere(headCheck.position, checkRadius, ceilingMask);

        // when starting crawling, collder 
        void DecreaseCollider()
        {
            controller.height = crawlHeight;
            controller.center = crawlCenter;

            if (capsuleCollider != null)
            {
                capsuleCollider.height = crawlHeight;
                capsuleCollider.center = crawlCenter;
            }
        }

        // when stoping crawling == stand up
        void DefaultCollider()
        {
            controller.height = originalHeight;
            controller.center = originalCenter;

            if (capsuleCollider != null)
            {
                capsuleCollider.height = originalCapsuleHeight;
                capsuleCollider.center = originalCapsuleCenter;
            }
        }


        // // function for checking there is ceiling upside
        // bool CanStandUp()
        // {
        //     return !Physics.CheckSphere(headCheck.position, checkRadius, ceilingMask);
        // }

        // For DEBUG, draw red circle checking there is ceiling object above head
        void OnDrawGizmosSelected()
        {
            if (headCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(headCheck.position, checkRadius);
            }
        }

        if (CrawlMethod == Custom2Method.Controller)
        {
            UpdateControlTypeUI("Controller");
            if (!isCrouching && crawlAction.GetStateDown(leftInputSource) && !isCrawling)
            {
                Debug.Log("Crawl Left Triggered");
                DecreaseCollider();
                animator.SetTrigger("Crawl Start");
                isCrawling = true;

                if (activeHand == leftInputSource)
                    activeHand = rightInputSource;
            }
            // if (!isCrouching && crawlAction.GetStateDown(rightInputSource) && !isCrawling)
            // {
            //     Debug.Log("Crawl Right Triggered");
            //     DecreaseCollider();
            //     animator.SetTrigger("Crawl Start");
            //     isCrawling = true;

            //     if (activeHand == rightInputSource)
            //         activeHand = leftInputSource;
            // }

            //HBD & FBD crawl and walk
            if (WalkMethod != Custom2Method.Controller)
            {
                if (isCrawling)
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
                
            }

            if ((crawlAction.GetStateUp(leftInputSource) && isCrawling) ||
                (crawlAction.GetStateUp(rightInputSource) && isCrawling))
            {
                if (canStand)
                {
                    DefaultCollider();
                    animator.SetTrigger("Crawl End");
                    isCrawling = false;
                }
            }
        }
        else if (CrawlMethod == Custom2Method.HalfBody)
        {
            UpdateControlTypeUI("Half Body");
            bool inCrawlRange = leftHandY > crawlThreshold1 && leftHandY < crawlThreshold2 &&
                                rightHandY > crawlThreshold1 && rightHandY < crawlThreshold2;
            if (idlecheck && inCrawlRange)
            {
                crawlConditionTimer += Time.deltaTime; // 조건 만족 중 → 타이머 증가

                if (crawlConditionTimer >= crawlHoldTimeRequired && !isCrawling)
                {
                    //Debug.Log("Crawl Triggered after Hold");
                    DecreaseCollider();
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
                if (handsAreCrossing && avgHandSpeed > 0.05f)
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
                if (walkTimer > 0)
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
                float leftDelta = Mathf.Abs(leftHandY - crawlStartLeftY);
                float rightDelta = Mathf.Abs(rightHandY - crawlStartRightY);

                // Debug.Log("left Delta: " + leftDelta);

                if (leftDelta > crawlExitDelta && rightDelta > crawlExitDelta)
                {
                    if (canStand)
                    {
                        DefaultCollider();
                        animator.SetTrigger("Crawl End");
                        isCrawling = false;
                    }
                }
            }
        }
        else
        {
            UpdateControlTypeUI("Full Body");
            // bool inCrawlRange = (headY - leftFootY) < 0.3f;
            // Debug.Log($"Crawl Check | LH-LF: {Mathf.Abs(leftHandY - leftFootY):F2}, RH-RF: {Mathf.Abs(rightHandY - rightFootY):F2}");

            if (idlecheck && /* headY < 0.6f && */ leftHandY < 0.2f && rightHandY < 0.2f)
            {
                crawlConditionTimer += Time.deltaTime; // 조건 만족 중 → 타이머 증가

                if (crawlConditionTimer >= crawlHoldTimeRequired && !isCrawling)
                {
                    DecreaseCollider();
                    animator.SetTrigger("Crawl Start");
                    isCrawling = true;
                    cameraRigAlignerStudy2.rootOffset = new Vector3(
                        cameraRigAlignerStudy2.rootOffset.x,
                        0f,
                        cameraRigAlignerStudy2.rootOffset.z
                    );
                    cameraRigAlignerStudy2.rootOffset = new Vector3(0.0f, 0.0f, 0.0f);

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
                if (handsAreCrossing && avgHandSpeed > 0.05f)
                {
                    Debug.Log("isCrawling true");
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
                if (walkTimer > 0)
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
                // float leftDelta = Mathf.Abs(leftHandY - crawlStartLeftY);
                // float rightDelta = Mathf.Abs(rightHandY - crawlStartRightY);

                // Debug.Log("left Delta: " + leftDelta);

                if (/* headY > 0.5f */ leftHandY > 0.5f && rightHandY > 0.5f && canStand)
                {
                    //Debug.Log("Crawl End triggered by movement delta");
                    DefaultCollider();
                    animator.SetTrigger("Crawl End");
                    isCrawling = false;
                    cameraRigAlignerStudy2.rootOffset = new Vector3(
                        cameraRigAlignerStudy2.rootOffset.x,
                        1f,
                        cameraRigAlignerStudy2.rootOffset.z
                    );
                    cameraRigAlignerStudy2.rootOffset = new Vector3(0.0f, 1.0f, 0.0f);
                }
            }
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
    private IEnumerator JumpProtectionDelay(float delay)
    {
        isJumpingProtected = true;
        yield return new WaitForSeconds(delay);
        isJumpingProtected = false;
        isJumping = false;
    }

    // Control Type UI Update
    public void UpdateControlTypeUI(string type)
    {
        test2Manager.ControlTypeUI.text = type;
    }
}