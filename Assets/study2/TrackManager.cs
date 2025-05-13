using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Transform player;              // 플레이어의 위치
    public List<GameObject> segments;     // 현재 활성화된 타일들
    private float segmentLength = 60f;     // 타일 하나의 길이
    public float segmentRespone = 90f;
    public int segmentCount = 3;          // 초기 타일 개수 (씬에 배치된 수)

    void Update()
    {
        if (segments.Count == 0) return;

        GameObject firstSegment = segments[0];

        // 플레이어가 첫 타일을 지나쳤으면
        if (player.position.z - firstSegment.transform.position.z > segmentRespone)
        {
            // 가장 앞 타일을 리스트에서 제거
            segments.RemoveAt(0);

            // 가장 뒤 타일 뒤쪽으로 이동
            GameObject lastSegment = segments[segments.Count - 1];
            firstSegment.transform.position = lastSegment.transform.position + Vector3.forward * segmentLength;

            // 리스트 마지막에 다시 추가
            segments.Add(firstSegment);
        }
    }
}
