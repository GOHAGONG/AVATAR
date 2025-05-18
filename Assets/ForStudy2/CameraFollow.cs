using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0f, 5f, -7f); // �Ÿ�/����
    public float followSpeed = 5f;     // ���󰡴� �ӵ�
    public float lookSpeed = 5f;       // ȸ�� �ӵ�

    void LateUpdate()
    {
        if (target == null) return;

        // �ε巴�� ��ġ �̵�
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // �ε巴�� ȸ���Ͽ� �÷��̾� �ٶ󺸱�
        //Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
