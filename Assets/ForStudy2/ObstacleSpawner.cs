using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool pool;
    public Transform spawnPoint;
    public Transform targetPoint;
    public Transform player;
    public float stopDistance = 1f; // 플레이어가 spawn에 도달하면 멈춤
    public float spawnInterval = 2f;
    public float moveSpeed = 5f;

    private float timer;
    private bool spawningStopped = false;

    void Update()
    {
        if (spawningStopped) return;

        // 플레이어가 spawnPoint에 가까이 가면 스폰 중단
        if (Vector3.Distance(player.position, spawnPoint.position) < stopDistance)
        {
            spawningStopped = true;
            Debug.Log("플레이어가 SpawnPoint에 도달했습니다. 장애물 스폰 중단!");
            return;
        }

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

        Obstacle obstacle = obj.GetComponent<Obstacle>();
        obstacle.Setup(pool, targetPoint.position, moveSpeed);
    }
}
