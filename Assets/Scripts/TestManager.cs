using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class TestManager : MonoBehaviour
{
    public enum ActionType
    {
        Walk,
        Run,
        Jump,
        Sit,
        Crawl
    }

    public enum ControlType
    {
        KeyboardMouse,
        Controller,
        HalfBody,
        FullBody
    }

    public enum TestState
    {
        WaitingForActionComplete,
        WaitingForSurveyInput,
        TestCompleted
    }

    [Header("실험 설정")] 
    public ActionType currentAction;
    public ControlType currentControl;
    
    public GameObject Target;
    
    private List<ActionType> currentTrialSequence;
    private int currentTrialIndex = 0;
    public TestState currentState = TestState.WaitingForActionComplete;

    [Header("UI")]
    public TMP_Text ControlTypeUI;
    public GameObject WalkUI;
    public GameObject RunUI;
    public GameObject SitUI;
    public GameObject CrawlUI;
    public GameObject JumpUI;
    public GameObject NASAFMS_UI;
    public GameObject SUSVRSQ_UI;

    private void Start()
    {
        UpdateControlTypeUI();
        currentTrialSequence = GenerateRandomizedSequence();
        currentTrialIndex = 0;
        StartNextTrial();
    }

    private void Update()
    {
        // Enter 누르면 NASA&FMS 설문 or 다음 동작 수행
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter");
            TryTransitionToSurvey();
        }
    }

    public void TryTransitionToSurvey()
    {
        if (currentState == TestState.WaitingForActionComplete)
        {
            ShowNASAFMS();
        }
        else if (currentState == TestState.WaitingForSurveyInput)
        {
            StartNextTrial();
        }
    }
    
    private void UpdateControlTypeUI()
    {
        string displayText = "";

        switch (currentControl)
        {
            case ControlType.KeyboardMouse:
                displayText = "Keyboard + Mouse";
                break;
            case ControlType.Controller:
                displayText = "Controller";
                break;
            case ControlType.HalfBody:
                displayText = "Half Body";
                break;
            case ControlType.FullBody:
                displayText = "Full Body";
                break;
        }

        ControlTypeUI.text = $"Control Type: {displayText}";
    }
    
    private List<ActionType> GenerateRandomizedSequence()
    {
        List<ActionType> result = new List<ActionType>();
        
        //동작 랜덤 시퀀스 생성
        var shuffled = Enum.GetValues(typeof(ActionType))
                                   .Cast<ActionType>()
                                   .OrderBy(x => UnityEngine.Random.value)
                                   .ToList();
        result.AddRange(shuffled);
        return result;
    }

    public void StartNextTrial()
    {
        HideAllUI();
        
        // target object off
        Target.SetActive(false);

        // 모든 동작 수행했을 시
        if (currentTrialIndex >= currentTrialSequence.Count)
        {
            StartPostControlSurvey();
            currentState = TestState.TestCompleted;
            return;
        }

        // 남은 동작 있을 시
        currentAction = currentTrialSequence[currentTrialIndex];
        currentTrialIndex++;

        StartAction(currentAction);
        currentState = TestState.WaitingForActionComplete;
    }

    public void StartAction(ActionType action)
    {
        HideAllUI();

        switch (action)
        {
            case ActionType.Walk:
                WalkUI.SetActive(true);
                
                // target object setting
                Target.SetActive(true);
                Target.GetComponent<Animation>().Play("walkTarget");
                break;
            case ActionType.Run:
                RunUI.SetActive(true);
                
                // target object setting
                Target.SetActive(true);
                Target.GetComponent<Animation>().Play("runTarget");
                break;
            case ActionType.Jump:
                JumpUI.SetActive(true);
                break;
            case ActionType.Sit:
                SitUI.SetActive(true);
                break;
            case ActionType.Crawl:
                CrawlUI.SetActive(true);
                
                // target object setting
                Target.SetActive(true);
                Target.GetComponent<Animation>().Play("crawlTarget");
                break;
        }
    }

    private void ShowNASAFMS()
    {
        HideAllUI();
        NASAFMS_UI.SetActive(true);
        currentState = TestState.WaitingForSurveyInput;
    }

    public void StartPostControlSurvey()
    {
        HideAllUI();
        SUSVRSQ_UI.SetActive(true);
        Debug.Log($"[{currentControl}] 모든 동작 완료, SUS & VRSQ 설문 시작");
    }

    private void HideAllUI()
    {
        WalkUI.SetActive(false);
        RunUI.SetActive(false);
        JumpUI.SetActive(false);
        SitUI.SetActive(false);
        CrawlUI.SetActive(false);
        NASAFMS_UI.SetActive(false);
        SUSVRSQ_UI.SetActive(false);
    }
}
