using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public TestManager testManager;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"[Trigger] 닿은 오브젝트 이름: {other.name}, 태그: {other.tag}");
        // "Player" 태그가 붙은 아바타와 접촉 시 설문으로 전환
        if (testManager.currentState == TestManager.TestState.WaitingForActionComplete && other.CompareTag("Player"))
        {
            testManager.TryTransitionToSurvey();
        }
    }
}