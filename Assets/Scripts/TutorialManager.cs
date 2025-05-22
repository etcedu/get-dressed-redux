using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class TutorialStep
{
    [TextArea]
    public string message;
    public string hintFingerTag; //optional
    public GameObject limitInteractionToThis; //optional
    public List<Button> lockedButtons; //optional
    public UnityEvent onStepStartEvent;
    public UnityEvent onStepFinishEvent;
    public float hintFingerDelayOverride = -1;
}

public class TutorialManager : MonoBehaviour
{
    public List<TutorialStep> steps = new List<TutorialStep>();
    int currentStepIndex;
    TutorialStep currentStep;
    HintFingerManager hintFingerManager;

    [SerializeField] TutorialPanel tutorialPanel;
    [SerializeField] float hintFingerDelayDefault = 3.0f;

    private void Start()
    {
        hintFingerManager = FindObjectOfType<HintFingerManager>();
    }

    [ContextMenu("Start Tutorial")]
    public void StartTutorial()
    {
        currentStep = steps[0];

        StartStep(currentStep);
    }

    void StartStep(TutorialStep step)
    {        
        if (hintFingerManager == null)            
            hintFingerManager = FindObjectOfType<HintFingerManager>();

        currentStep = step;

        step.onStepStartEvent.Invoke();
        if (!string.IsNullOrEmpty(step.message))
            tutorialPanel.Show(step.message);


        if (step.limitInteractionToThis != null)
        {
            EventSystem.current.limitInputToThisObject = step.limitInteractionToThis;
            //EventSystem.current.eventOnLimitedInputInteraction += EndStep;

            Debug.Log("ADded listener");
            step.limitInteractionToThis.GetComponentInChildren<Button>()?.onClick.AddListener(EndStep);
            step.limitInteractionToThis.GetComponentInChildren<Toggle>()?.onValueChanged.AddListener(EndStep);
        }

        if (step.lockedButtons != null)
        {
            foreach (Button b in step.lockedButtons)
                b.interactable = false;
        }
        
        if (!string.IsNullOrEmpty(step.hintFingerTag))
            hintFingerManager.ShowHintOnTimer(step.hintFingerTag, step.hintFingerDelayOverride > 0 ? step.hintFingerDelayOverride : hintFingerDelayDefault);
    }

    void EndStep(bool _)
    {
        EndStep();
    }

    void EndStep()
    {
        SimpleRTVoiceExample.Instance.StopSpeech();

        if (hintFingerManager == null)
            hintFingerManager = FindObjectOfType<HintFingerManager>();

        hintFingerManager.HideHintFinger(currentStep.hintFingerTag);
        currentStep.onStepFinishEvent.Invoke();

        if (currentStep.limitInteractionToThis != null)
        {
            currentStep.limitInteractionToThis.GetComponentInChildren<Button>()?.onClick.RemoveListener(EndStep);
            currentStep.limitInteractionToThis.GetComponentInChildren<Toggle>()?.onValueChanged.RemoveListener(EndStep);
        }

        EventSystem.current.limitInputToThisObject = null;
        EventSystem.current.eventOnLimitedInputInteraction -= EndStep;
        if (currentStep.lockedButtons != null)
        {
            foreach (Button b in currentStep.lockedButtons)
                b.interactable = true;
        }
        tutorialPanel.Hide();

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
