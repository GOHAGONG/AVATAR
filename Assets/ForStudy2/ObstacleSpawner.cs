using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool pool;
    public Transform spawnPoint;
    public Transform targetPoint; // ��ǥ ����
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
            Debug.LogWarning("SpawnPoint �Ǵ� TargetPoint�� �������� �ʾҽ��ϴ�.");
            return;
        }

        GameObject obj = pool.GetFromPool();
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.LookRotation(targetPoint.position - spawnPoint.position);

        // Obstacle ��ũ��Ʈ�� �̵� ����� �ӵ� ����
        Obstacle obstacle = obj.GetComponent<Obstacle>();
        obstacle.Setup(pool, targetPoint.position, moveSpeed);
    }
}
