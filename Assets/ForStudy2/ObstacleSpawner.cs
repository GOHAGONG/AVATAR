using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool pool;
    public Transform spawnPoint;
    public Transform targetPoint; // 목표 지점
    public float spawnInterval = 2f;
    public float moveSpeed = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        if (spawnPoint == null || targetPoint == null)
        {
            Debug.LogWarning("SpawnPoint 또는 TargetPoint가 설정되지 않았습니다.");
            return;
        }

        GameObject obj = pool.GetFromPool();
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.LookRotation(targetPoint.position - spawnPoint.position);

        // Obstacle 스크립트에 이동 방향과 속도 전달
        Obstacle obstacle = obj.GetComponent<Obstacle>();
        obstacle.Setup(pool, targetPoint.position, moveSpeed);
    }
}
