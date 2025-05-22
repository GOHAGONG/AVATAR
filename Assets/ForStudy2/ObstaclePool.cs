using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // ���� ������
    public int poolSize = 5;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        if (obstaclePrefabs.Length == 0) return;

        int count = Mathf.Min(poolSize, obstaclePrefabs.Length);

        // 최소 1개씩 넣되, poolSize보다 작으면 일부만 넣기
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(obstaclePrefabs[i]);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        // 남은 수만큼 랜덤 추가
        for (int i = count; i < poolSize; i++)
        {
            GameObject prefab = GetRandomPrefab();
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }


    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        GameObject prefab = GetRandomPrefab();
        return Instantiate(prefab);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private GameObject GetRandomPrefab()
    {
        if (obstaclePrefabs.Length == 0) return null;
        return obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
    }
}
