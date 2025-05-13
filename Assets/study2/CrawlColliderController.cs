using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CrawlColliderController : MonoBehaviour
{
    private CharacterController controller;
    private float originalHeight;
    private Vector3 originalCenter;

    [Header("Crawl Settings")]
    public float crawlHeight = 1.0f;
    public Vector3 crawlCenter = new Vector3(0f, 0.5f, 0f);

    private bool isCrawling = false;

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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
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
                animator.SetBool("isCrawling", true);
                Debug.Log("Cannot stand up: ceiling too low. Staying in crawl.");
            }
        }
    }

    void StartCrawl()
    {
        isCrawling = true;
        controller.height = crawlHeight;
        controller.center = crawlCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl Start");
        }
    }

    void StopCrawl()
    {
        isCrawling = false;
        controller.height = originalHeight;
        controller.center = originalCenter;

        if (animator != null)
        {
            animator.SetTrigger("Crawl End");
        }
    }

    bool CanStandUp()
    {
        float radius = controller.radius;
        float checkDistance = originalHeight / 2f;
        Vector3 start = transform.position + Vector3.up * crawlCenter.y;
        Vector3 end = transform.position + Vector3.up * (checkDistance + 0.1f); // 약간 여유

        // 환경 레이어 (Ground나 Environment 등) 설정에 맞게 변경
        int layerMask = LayerMask.GetMask("Celling");

        return !Physics.CheckCapsule(start, end, radius, layerMask);
    }
}
