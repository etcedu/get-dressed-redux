using System.Collections.Generic;
using SimcoachGames.EventRecorder;
using UnityEngine;

public class ParentalGateHighestNumberChallenge : ParentalGateChallengeBase
{
    [Header("Highest Number Settings")]
    [SerializeField] int lowestNum;
    [SerializeField] int highestNum;
    [SerializeField] int numAnswers;

    void Start()
    {
        List<(string, bool)> answers = new();
        int num = 0;
        for (int i = 0; i < numAnswers - 1; i++)
        {
            num += GetRandomUniqueNumber(lowestNum, highestNum);
            answers.Add((num.ToString(), false));
        }
        
        num += GetRandomUniqueNumber(lowestNum, highestNum);
        answers.Add((num.ToString(), true));
        
        EasySetAnswers(answers);
    }
}
