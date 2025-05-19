using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    private Vector3 targetPos;
    private float moveSpeed;
    private ObstaclePool pool;
    private bool isHit = false;

    public void Setup(ObstaclePool pool, Vector3 targetPos, float moveSpeed)
    {
        this.pool = pool;
        this.targetPos = targetPos;
        this.moveSpeed = moveSpeed;
        isHit = false;
        StopAllCoroutines(); // 이전 깜빡임 중지
    }

    void Update()
    {
        if (isHit) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            pool.ReturnToPool(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isHit) return;

        if (other.CompareTag("Player"))
        {
            isHit = true;
            StartCoroutine(BlinkAndReturn());
        }
    }

    IEnumerator BlinkAndReturn()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) yield break;

        for (int i = 0; i < 5; i++)
        {
            rend.enabled = false;
            yield return new WaitForSeconds(0.1f);
            rend.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        pool.ReturnToPool(gameObject);
    }
}
