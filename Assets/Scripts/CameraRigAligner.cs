using UnityEngine;
using System.Collections;
using Valve.VR;

public class CameraRigAligner : MonoBehaviour
{
    [Header("CameraRig 및 VR 기기 참조")]
    public Transform cameraRigRoot;              // CameraRig 루트 (이동 대상)
    public Transform hmdTransform;               // SteamVR Camera (head)
    public Transform leftControllerTransform;    // 왼쪽 컨트롤러
    public Transform rightControllerTransform;   // 오른쪽 컨트롤러
    public Transform leftFootTrackerTransform;       // 왼쪽 트래커
    public Transform rightFootTrackerTransform;      // 오른쪽 트래커
    // public Transform rightKneeTrackerTransform;      // 오른쪽 트래커

    [Header("아바타 본 위치")]
    public Transform avatarHead;
    public Transform avatarLeftHand;
    public Transform avatarRightHand;
    // public Transform avatarLeftKnee;
    // public Transform avatarRightKnee;
    public Transform avatarLeftFoot;
    public Transform avatarRightFoot;

    [Header("CameraRig와 아바타 루트 동기화")]
    public Transform avatarRoot;
    public Vector3 rootOffset;

    [Header("옵션: 손 위치 미세 보정")]
    public Vector3 leftHandOffset = Vector3.zero;
    public Vector3 rightHandOffset = Vector3.zero;

    [Header("옵션: 무릎 위치 미세 보정")]
    // public Vector3 leftKneeOffset = Vector3.zero;
    public Vector3 rightKneeOffset = Vector3.zero;
    [Header("옵션: 발 위치 미세 보정")]
    public Vector3 leftFootOffset = Vector3.zero;
    public Vector3 rightFootOffset = Vector3.zero;

    void Start()
    {
        // HMD와 아바타 머리의 현재 위치 차이를 계산
        Vector3 delta = avatarHead.position - hmdTransform.position;

        // CameraRig 전체를 이동시켜 HMD 위치를 아바타 머리에 정렬
        // cameraRigRoot.position += delta;

        // 선택적으로 회전도 맞추고 싶으면 아래 코드 주석 해제
        /*
        Quaternion rotDelta = Quaternion.FromToRotation(hmdTransform.forward, avatarHead.forward);
        cameraRigRoot.rotation = rotDelta * cameraRigRoot.rotation;
        */
    }

    void Update()
    {
        // [1] CameraRig을 아바타 머리에 정렬
        Vector3 delta = avatarHead.position - hmdTransform.position;
        Vector3 delta_prime = new Vector3(delta.x, 0.0f, delta.z);
        cameraRigRoot.position += delta_prime;

        // [2] 아바타 본들의 위치를 VR 기기 위치에 동기화
        avatarHead.position = hmdTransform.position;
        avatarHead.rotation = hmdTransform.rotation;

        // Hand
        avatarLeftHand.position = leftControllerTransform.position + leftHandOffset;
        avatarLeftHand.rotation = leftControllerTransform.rotation;

        avatarRightHand.position = rightControllerTransform.position + rightHandOffset;
        avatarRightHand.rotation = rightControllerTransform.rotation;

        // Knee
        // avatarLeftKnee.position = leftTrackerTransform.position + leftKneeOffset;
        // avatarLeftKnee.rotation = leftTrackerTransform.rotation;

        // avatarRightKnee.position = rightKneeTrackerTransform.position + rightKneeOffset;
        // avatarRightKnee.rotation = rightKneeTrackerTransform.rotation;

        // Foot
        avatarLeftFoot.position = leftFootTrackerTransform.position + leftFootOffset;
        avatarLeftFoot.rotation = leftFootTrackerTransform.rotation;

        avatarRightFoot.position = rightFootTrackerTransform.position + rightFootOffset;
        avatarRightFoot.rotation = rightFootTrackerTransform.rotation;
    }

    // hmd가 내려갔는데 아바타 다리가 올라오는 문제 해결
    void LateUpdate()
    {
        avatarRoot.position = transform.position + rootOffset;
    }
}
