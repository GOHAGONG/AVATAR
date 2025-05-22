using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool pool;
    public Transform spawnPoint;
    public Transform targetPoint;
    public Transform player;
    public float stopDistance = 1f; // �÷��̾ spawn�� �����ϸ� ����
    public float spawnInterval = 2f;
    public float moveSpeed = 5f;

    private float timer;
    private bool spawningStopped = false;

    void Update()
    {
        if (spawningStopped) return;

        // �÷��̾ spawnPoint�� ������ ���� ���� �ߴ�
        if (Vector3.Distance(player.position, spawnPoint.position) < stopDistance)
        {
            spawningStopped = true;
            Debug.Log("�÷��̾ SpawnPoint�� �����߽��ϴ�. ��ֹ� ���� �ߴ�!");
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
            Debug.LogWarning("SpawnPoint �Ǵ� TargetPoint�� �������� �ʾҽ��ϴ�.");
            return;
        }

        GameObject obj = pool.GetFromPool();
        obj.transform.position = spawnPoint.position;

        Obstacle obstacle = obj.GetComponent<Obstacle>();
        obstacle.Setup(pool, targetPoint.position, moveSpeed);
    }
}
