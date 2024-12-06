using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SimcoachGames.EventRecorder
{
    public class ParentalGateChallengeBase : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI promptTextUI;
        [SerializeField] Button buttonTemplate; 

        Action _successCallback;
        Action _failCallback;

        bool _answerEntered = false;
        
        void Awake() => buttonTemplate.gameObject.SetActive(false);

        public void Init(Action successCallback, Action failCallback)
        {
            _successCallback = successCallback;
            _failCallback = failCallback;
        }

        void ReportSuccess()
        {
            if (!_answerEntered)
            {
                _successCallback.Invoke();
                _answerEntered = true;
            }
        }

        void ReportFailed()
        {
            if (!_answerEntered)
            {
                _failCallback.Invoke();
                _answerEntered = true;
            }
        }

        protected void SetPromptText(string text)
        {
            promptTextUI.text = text;
        }

        protected void EasySetAnswers(List<(string, bool)> answers)
        {
            answers.Shuffle();
            foreach ((string, bool) answer in answers)
            {
                Button newButton = Instantiate(buttonTemplate, Vector3.zero, Quaternion.identity, buttonTemplate.transform.parent);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = answer.Item1;
                newButton.onClick.AddListener(() =>
                {
                    if (answer.Item2)
                    {
                        ReportSuccess();
                    }
                    else
                    {
                        ReportFailed();
                    }
                });
                newButton.gameObject.SetActive(true);
            }
        }

        readonly List<int> _randomNumbers = new();
        protected int GetRandomUniqueNumber(int rangeLow, int rangeHigh)
        {
            int num = Random.Range(rangeLow, rangeHigh);
            while (_randomNumbers.Contains(num)) 
                num = Random.Range(rangeLow, rangeHigh);
            
            _randomNumbers.Add(num);
            return num;
        }
    }
}