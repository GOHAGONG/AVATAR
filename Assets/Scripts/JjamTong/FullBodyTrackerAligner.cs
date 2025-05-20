using UnityEngine;
using Valve.VR;
using System.Collections;

public class FullBodyTrackerAligner : MonoBehaviour
{
    [Header("CameraRig 및 트래킹 기기")]
    public Transform cameraRigRoot;              // 플레이어 전체 Rig 이동 기준
    public Transform hmdTransform;               // HMD (Head)
    // public Transform leftControllerTransform;    // 왼손 컨트롤러
    // public Transform rightControllerTransform;   // 오른손 컨트롤러
    // public Transform leftFootTrackerTransform;   // 왼발 트래커
    // public Transform rightFootTrackerTransform;  // 오른발 트래커

    [Header("아바타 본 위치")]
    public Transform avatarHead;
    // public Transform avatarLeftHand;
    // public Transform avatarRightHand;
    // public Transform avatarLeftFoot;
    // public Transform avatarRightFoot;
    // public Transform avatarPelvis;

    // [Header("위치 오프셋")]
    public Vector3 headOffset;
    // public Vector3 leftHandOffset;
    // public Vector3 rightHandOffset;
    // public Vector3 leftFootOffset;
    // public Vector3 rightFootOffset;
    // public Vector3 pelvisOffset;

    void Start()
    {
        // 초기 정렬 (선택): HMD와 아바타 머리 위치 차이만큼 Rig 이동
        Vector3 delta = avatarHead.position - hmdTransform.position;
        //cameraRigRoot.position += new Vector3(delta.x, 0, delta.z); // Y는 제외 (회전 고려 가능)
    }

    void Update()
    {
        AlignAvatarToTrackedDevices();
    }

    private void AlignAvatarToTrackedDevices()
    {
        Vector3 delta = avatarHead.position - hmdTransform.position;
        cameraRigRoot.position += delta;
        
        // 1. HMD → Head
        avatarHead.position = hmdTransform.position + headOffset;
        avatarHead.rotation = hmdTransform.rotation;

        // // 2. Controllers → Hands
        // avatarLeftHand.position = leftControllerTransform.position + leftHandOffset;
        // avatarLeftHand.rotation = leftControllerTransform.rotation;

        // avatarRightHand.position = rightControllerTransform.position + rightHandOffset;
        // avatarRightHand.rotation = rightControllerTransform.rotation;

        // // 3. Foot Trackers → Feet
        // avatarLeftFoot.position = leftFootTrackerTransform.position + leftFootOffset;
        // avatarLeftFoot.rotation = leftFootTrackerTransform.rotation;

        // avatarRightFoot.position = rightFootTrackerTransform.position + rightFootOffset;
        // avatarRightFoot.rotation = rightFootTrackerTransform.rotation;

        // // 4. Pelvis 보정 (발 높이 평균 + 오프셋)
        // float avgFootY = (leftFootTrackerTransform.position.y + rightFootTrackerTransform.position.y) / 2f;
        // Vector3 pelvisPos = avatarPelvis.position;
        // pelvisPos.y = avgFootY + pelvisOffset.y;
        // avatarPelvis.position = pelvisPos;
    }
}
