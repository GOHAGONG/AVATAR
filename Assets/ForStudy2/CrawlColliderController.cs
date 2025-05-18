using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CrawlColliderController : MonoBehaviour
{
    private CharacterController controller;
    private float originalHeight;
    private Vector3 originalCenter;

    public Transform headCheck;
    public LayerMask ceilingMask;
    public float checkRadius = 0.3f;

    [Header("Crawl Settings")]
    public float crawlHeight = 1.0f;
    public Vector3 crawlCenter = new Vector3(0f, 0.5f, 0f);

    public bool isCrawling = false;

    [Header("Animation (Optional)")]
    public Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrawling)
        {
            isCrawling = true;
            StartCrawl();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isCrawling)
        {
            if (CanStandUp())
            {
                StopCrawl();
            }
            else
            {
                isCrawling = true;
                animator.SetBool("isCrawling", true);
                Debug.Log("Cannot stand up: ceiling too low. Staying in crawl.");
            }
        }
    }

    void StartCrawl()
    {
        controller.height = crawlHeight;
        controller.center = crawlCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl Start");
            animator.SetBool("isCrawling", true);
        }
    }

    void StopCrawl()
    {
        controller.height = originalHeight;
        controller.center = originalCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl End");
            animator.SetBool("isCrawling", false);
            isCrawling = false;
        }
    }

    bool CanStandUp()
    {
        float radius = checkRadius;
        float checkDistance = originalHeight / 2f;

        return !Physics.CheckSphere(headCheck.position, checkRadius, ceilingMask);
    }

    void OnDrawGizmosSelected()
    {
        if (headCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headCheck.position, checkRadius);
        }
    }
}
