using System;
using System.Collections.Generic;
using SimcoachGames.EventRecorder;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

public class ParentalGateBasicMathChallenge : ParentalGateChallengeBase
{
    [Header("Math Difficulty Settings")]
    [SerializeField] int lowestNum;
    [SerializeField] int highestNum;
    [SerializeField] int numDistractors;

    int _number1;
    int _number2;
    int _correctAnswer; 

    void Start()
    {
        _number1 = Random.Range(lowestNum, highestNum);
        _number2 = Random.Range(lowestNum, highestNum);
        _correctAnswer = _number1 + _number2;

        SetPromptText($"{_number1} + {_number2} = ?");

        List<(string, bool)> answers = new() {(_correctAnswer.ToString(), true)};
        for (int i = 0; i < numDistractors; i++)
        {
            int num = GetRandomUniqueNumber(lowestNum, highestNum);
            while (num == _correctAnswer) 
                num = GetRandomUniqueNumber(lowestNum, highestNum);
            
            answers.Add((num.ToString(), false));
        }
        
        EasySetAnswers(answers);
    }
}
