using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public static class CrossSceneInfo
{
	
	//WTP NOTE: EventRecorder related values
	public static Guid LevelAttemptId; //Hash value generated on each level to help associate start/complete events
	public static DateTime LevelStartedTimeStamp;
	public static string LastCompanyName;
	public static string LastPositionName;
	
	

	public static CompanyPositionInfo SelectedCompany;
	public static Clothing.Tier.TierEnum? InterviewTier;

	private const int interviewPassedCutoff = 400;
	public static int PassingCuttoff
	{
		get{ return interviewPassedCutoff; }
	}

	#region Score
	/// <summary>
	/// Get the total score for all interviews completed.
	/// </summary>
	public static int TotalScore
	{
		get{
			
			return _totalScore;
		}
	}
	/// <summary>
	/// DO NOT USE DIRECTLY. Reference TotalScore instead
	/// </summary>
	private static int _totalScore;
	/// <summary>
	/// Flag for recalculating the total score
	/// </summary>
	private static bool _totalScoreChanged = true;
	/// <summary>
	/// Set the score (value) for the interview (id)
	/// </summary>
	public static void SetScore(string id, int value)
	{
        //SendHiredGameData();
		
	}

    private static void SendHiredGameData()
    {
        
    }

    #endregion

	
	public enum GenderEnum
	{
		MALE, FEMALE
	}
}
