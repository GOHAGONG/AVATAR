using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class Test2Manager : MonoBehaviour
{
    public enum ActionType
    {
        WalkRun,
        CrouchJump,
        Crawl
    }

    public enum ControlType
    {
        KeyboardMouse,
        Controller,
        HalfBody,
        FullBody
    }

    [Header("실험 설정")] 
    public ActionType currentAction;
    public ControlType currentControl;
    
    private List<ActionType> currentTrialSequence;
    private int currentTrialIndex = 0;

    [Header("UI")]
    public TMP_Text ControlTypeUI;
    public GameObject WalkRunUI;
    public GameObject CrouchJumpUI;
    public GameObject CrawlUI;
    public GameObject SUSVRSQ_UI;

    private void Start()
    {
        currentTrialSequence = new List<ActionType>
        {
            ActionType.WalkRun,
            ActionType.Crawl,
            ActionType.CrouchJump
        };
        
        currentTrialIndex = 0;
        StartNextTrial();
    }

    public void OnPlayerReachedTrigger()
    {
        StartNextTrial(); // 바로 다음 동작으로 넘어감
    }

    public void StartNextTrial()
    {
        HideAllUI();

        // 모든 동작 수행했을 시
        if (currentTrialIndex >= currentTrialSequence.Count)
        {
            StartPostControlSurvey();
            return;
        }

        // 남은 동작 있을 시
        currentAction = currentTrialSequence[currentTrialIndex];
        currentTrialIndex++;

        StartAction(currentAction);
    }

    public void StartAction(ActionType action)
    {
        HideAllUI();

        switch (action)
        {
            case ActionType.WalkRun:
                WalkRunUI.SetActive(true);
                break;
            case ActionType.CrouchJump:
                CrouchJumpUI.SetActive(true);
                break;
            case ActionType.Crawl:
                CrawlUI.SetActive(true);
                break;
        }
    }

    public void StartPostControlSurvey()
    {
        HideAllUI();
        SUSVRSQ_UI.SetActive(true);
        Debug.Log($"[{currentControl}] 모든 동작 완료, SUS & VRSQ 설문 시작");
    }

    private void HideAllUI()
    {
        WalkRunUI.SetActive(false);
        CrouchJumpUI.SetActive(false);
        CrawlUI.SetActive(false);
        SUSVRSQ_UI.SetActive(false);
    }
}
