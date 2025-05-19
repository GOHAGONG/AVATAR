using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Transform player;              // �÷��̾��� ��ġ
    public List<GameObject> segments;     // ���� Ȱ��ȭ�� Ÿ�ϵ�
    private float segmentLength = 60f;     // Ÿ�� �ϳ��� ����
    public float segmentRespone = 90f;
    public int segmentCount = 3;          // �ʱ� Ÿ�� ���� (���� ��ġ�� ��)

    void Update()
    {
        if (segments.Count == 0) return;

        GameObject firstSegment = segments[0];

        // �÷��̾ ù Ÿ���� ����������
        if (player.position.z - firstSegment.transform.position.z > segmentRespone)
        {
            // ���� �� Ÿ���� ����Ʈ���� ����
            segments.RemoveAt(0);

            // ���� �� Ÿ�� �������� �̵�
            GameObject lastSegment = segments[segments.Count - 1];
            firstSegment.transform.position = lastSegment.transform.position + Vector3.forward * segmentLength;

            // ����Ʈ �������� �ٽ� �߰�
            segments.Add(firstSegment);
        }
    }
}
