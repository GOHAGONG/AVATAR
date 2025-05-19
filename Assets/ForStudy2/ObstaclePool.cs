using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // ¿©·¯ ÇÁ¸®ÆÕ
    public int poolSize = 5;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
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
