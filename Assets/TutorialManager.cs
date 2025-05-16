using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class TutorialStep
{
    [TextArea]
    public string message;
    public string hintFingerTag; //optional
    public GameObject limitInteractionToThis; //optional
    public UnityEvent onStepStartEvent;
    public UnityEvent onStepFinishEvent;
}

public class TutorialManager : MonoBehaviour
{
    public List<TutorialStep> steps = new List<TutorialStep>();
    int currentStepIndex;
    TutorialStep currentStep;

    [SerializeField] TutorialPanel tutorialPanel;

    [ContextMenu("Start Tutorial")]
    public void StartTutorial()
    {
        currentStep = steps[0];

        StartStep(currentStep);
    }

    void StartStep(TutorialStep step)
    {        
        currentStep = step;

        step.onStepStartEvent.Invoke();
        if (!string.IsNullOrEmpty(step.message))
            tutorialPanel.Show(step.message);

        if (step.limitInteractionToThis != null)
        {
            EventSystem.current.limitInputToThisObject = step.limitInteractionToThis;
            EventSystem.current.eventOnLimitedInputInteraction += EndStep;
        }
        
        if (!string.IsNullOrEmpty(step.hintFingerTag))
            HintFingerManager.ShowHintFinger(step.hintFingerTag);
    }

    void EndStep()
    {
        HintFingerManager.HideHintFinger(currentStep.hintFingerTag);
        currentStep.onStepFinishEvent.Invoke();
        EventSystem.current.limitInputToThisObject = null;
        EventSystem.current.eventOnLimitedInputInteraction -= EndStep;

        currentStepIndex++;
        if (currentStepIndex < steps.Count)
        {
            currentStep = steps[currentStepIndex];
            StartStep(currentStep);
        }
        else
        {
            currentStep = null;
            EndTutorial();
        }
    }

    void EndTutorial()
    {
        tutorialPanel.Hide();
        EventSystem.current.limitInputToThisObject = null;
    }
}
