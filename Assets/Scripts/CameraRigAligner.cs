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
    public Transform leftTrackerTransform;       // 왼쪽 트래커
    public Transform rightTrackerTransform;      // 오른쪽 트래커

    [Header("아바타 본 위치")]
    public Transform avatarHead;
    public Transform avatarLeftHand;
    public Transform avatarRightHand;
    public Transform avatarLeftElbow;
    public Transform avatarRightElbow;
    public Transform avatarLeftFoot;
    public Transform avatarRightFoot;

    [Header("옵션: 손 위치 미세 보정")]
    public Vector3 leftHandOffset = Vector3.zero;
    public Vector3 rightHandOffset = Vector3.zero;

    [Header("옵션: 팔꿈치 위치 미세 보정")]
    public Vector3 leftElbowOffset = Vector3.zero;
    public Vector3 rightElbowOffset = Vector3.zero;
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
        cameraRigRoot.position += delta;

        // [2] 아바타 본들의 위치를 VR 기기 위치에 동기화
        avatarHead.position = hmdTransform.position;
        avatarHead.rotation = hmdTransform.rotation;

        // Hand
        avatarLeftHand.position = leftControllerTransform.position + leftHandOffset;
        avatarLeftHand.rotation = leftControllerTransform.rotation;

        avatarRightHand.position = rightControllerTransform.position + rightHandOffset;
        avatarRightHand.rotation = rightControllerTransform.rotation;

        // Elbow
        // avatarLeftElbow.position = leftTrackerTransform.position + leftElbowOffset;
        // avatarLeftElbow.rotation = leftTrackerTransform.rotation;

        // avatarRightElbow.position = rightTrackerTransform.position + rightElbowOffset;
        // avatarRightElbow.rotation = rightTrackerTransform.rotation;

        // Foot
        // avatarLeftFoot.position = leftTrackerTransform.position + leftFootOffset;
        // avatarLeftFoot.rotation = leftTrackerTransform.rotation;

        // avatarRightFoot.position = rightTrackerTransform.position + rightFootOffset;
        // avatarRightFoot.rotation = rightTrackerTransform.rotation;
    }
}
