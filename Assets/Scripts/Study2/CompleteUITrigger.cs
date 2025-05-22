using UnityEngine;

public class CompleteUITrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 또는 VR Rig에 맞게 수정
        {
            FindObjectOfType<Test2Manager>().StartPostControlSurvey();
        }
    }
}
