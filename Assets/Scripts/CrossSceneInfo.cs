using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Simcoach.SkillArcade;

public static class CrossSceneInfo
{
	
	//WTP NOTE: EventRecorder related values
	public static Guid LevelAttemptId; //Hash value generated on each level to help associate start/complete events
	public static DateTime LevelStartedTimeStamp;
	public static string LastCompanyName;
	public static string LastPositionName;
	
	
	#region Gender
	/// <summary>
	/// Gets the current gender, loading it from save if necessary
	/// </summary>
	public static GenderEnum Gender
	{
		get
		{
			if(!_genderLoaded)
				_gender = (GameBase.Strings.GetValue("Gender", "Male") == "Male") ? GenderEnum.MALE : GenderEnum.FEMALE;
			return _gender;
		}
		set
		{
			_gender = value;
			GameBase.Strings.SetValue("Gender", (value == GenderEnum.MALE) ? "Male" : "Female");
			GameBase.Strings.Save();
		}
	}
	/// <summary>
	/// DO NOT REFERENCE DIRECTLY. Use Gender instead.
	/// </summary>
	private static GenderEnum _gender;
	/// <summary>
	/// Flag for loading gender
	/// </summary>
	private static bool _genderLoaded = false;
	#endregion
	
	public static CompanyPositionInfo SelectedCompany;
	public static Clothing.Tier.TierEnum? InterviewTier;

	private const int interviewPassedCutoff = 400;
	public static int PassingCuttoff
	{
		get{ return interviewPassedCutoff; }
	}

	#region Rank
	/// <summary>
	/// Gets the current rank, based on the total score relative to the rank cutoffs
	/// </summary>
	public static int Rank
	{
		get{
			if(_checkRank)
			{
				_checkRank = false;
				bool hasPreviousRank = _rank.HasValue;
				int previousRank = -1;
				if(hasPreviousRank)
					previousRank = _rank.Value;
				_rank = 1;
				for(_rank = 1; _rank < _rankCutoffs.Length; _rank++)
					if(_rankCutoffs[_rank.Value] > TotalScore)
						break;
				_rank--;
				if(hasPreviousRank && previousRank != _rank.Value)
					RankChanged = true;
			}
/*			Debug.Log("Rank: " + _rank);
*/			return _rank.Value;
		}
	}
	/// <summary>
	/// DO NOT REFERENCE DIRECTLY. Use Rank instead.
	/// </summary>
	private static int? _rank;
	/// <summary>
	/// Flag for checking the rank (ex. used after total score is updated)
	/// </summary>
	private static bool _checkRank = true;
	/// <summary>
	/// Total score cutoffs for rank levels
	/// </summary>
	private static int[] _rankCutoffs = new int[]
	{
		0, 2000, 4000, 6000, 8000
	};
	public static bool RankChanged
	{
		get{ return GameBase.Bools.GetValue("RankChanged", false); }
		private set{ GameBase.Bools.SetValue("RankChanged", value);  GameBase.SaveAll(); }
	}
	public static void RankChangeReset()
	{
		GameBase.Bools.SetValue("RankChanged", false);
		GameBase.SaveAll();
	}
	/// <summary>
	/// Returns the score necessary to qualify for the specified rank level
	/// </summary>
	public static int GetRankCutoff(int rank)
	{
		return _rankCutoffs[rank];
	}
	/// <summary>
	/// Returns the name of the current rank, based on the total score
	/// </summary>
	public static string RankName
	{
		get
		{
			return _rankNames[Rank];
		}
	}
	/// <summary>
	/// The rank names.
	/// </summary>
	private static string[] _rankNames = new string[]
	{
		"Trainee", "Entry Level", "Experienced", "Pro", "JobPro"
	};
	public static string GetRankName(int rank)
	{
		return _rankNames[rank];
	}
	#endregion


	#region Score
	/// <summary>
	/// Get the total score for all interviews completed.
	/// </summary>
	public static int TotalScore
	{
		get{
			if(_totalScoreChanged)
			{
				_totalScoreChanged = false;
				_totalScore = 0;
				foreach(string level in CompletedInterviews)
					_totalScore += GameBase.Ints.GetValue(level);
			}
/*			Debug.Log("Total Score: " + _totalScore);
*/			return _totalScore;
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
		if(GameBase.Ints.GetValue(id) < value)
		{
			_totalScoreChanged = true;
			_checkRank = true;
			GameBase.Ints.SetValue(id, value);
			if(!CompletedInterviews.Contains(id))
			{
				CompletedInterviews = CompletedInterviews.AddAndReturn(id);
			}
			if(!PassedInterviews.Contains(id) && value >= interviewPassedCutoff)
			{
				PassedInterviews = PassedInterviews.AddAndReturn(id);
			}
			GameBase.SaveAll();
			int i = TotalScore;
			i = Rank;
		}
	}

    private static void SendHiredGameData()
    {
        List<Simcoach.SkillArcade.Pair<string, string>> gameData = new List<Simcoach.SkillArcade.Pair<string, string>>();
        gameData.Add(new Simcoach.SkillArcade.Pair<string, string>("timesHired", 1.ToString()));
        EventManagerBase.SendDataEvent("numberOfTimesHired", gameData);
    }

    #endregion


    #region Interviews
    /// <summary>
    /// List of all completed interviews
    /// </summary>
    public static List<string> CompletedInterviews
	{
		get{
			if(!_completedInterviewsLoaded)
			{
				_completedInterviewsLoaded = true;
/*				Debug.Log("loading levels");
*/				_completedInterviews = GameBase.StringArrays.GetValue("CompletedInterviews").ToList();
			}
			return _completedInterviews;
		}
		private set{
			_completedInterviews = value;
			GameBase.StringArrays.SetValue("CompletedInterviews", _completedInterviews.ToArray());
		}
	}
	/// <summary>
	/// DO NOT USE DIRECTLY. Reference CompletedInterviews
	/// </summary>
	private static List<string> _completedInterviews;
	/// <summary>
	/// Flag for loading the completed interviews
	/// </summary>
	private static bool _completedInterviewsLoaded = false;

	/// <summary>
	/// List of all passed interviews
	/// </summary>
	public static List<string> PassedInterviews
	{
		get{
			if(!_passedInterviewsLoaded)
			{
				_passedInterviewsLoaded = true;
				_passedInterviews = GameBase.StringArrays.GetValue("PassedInterviews").ToList();
			}
			return _passedInterviews;
		}
		private set{
			_passedInterviews = value;
			GameBase.StringArrays.SetValue("PassedInterviews", _passedInterviews.ToArray());
		}
	}
	/// <summary>
	/// DO NOT USE DIRECTLY. Reference PassedInterviews instead
	/// </summary>
	private static List<string> _passedInterviews;
	/// <summary>
	/// Flag for loading the completed interviews
	/// </summary>
	private static bool _passedInterviewsLoaded = false;
	#endregion


	#region Color
	public static string CharacterColorHex
	{
		get{ return GameBase.Strings.GetValue("CharacterColor", DefaultCharacterColor.ToHexStringRGBA()); }
		set{
			GameBase.Strings.SetValue("CharacterColor", value);
			GameBase.Strings.Save();
		}
	}
	public static Color CharacterColor
	{
		get{
			Color32 retColor = DefaultCharacterColor;
			ColorExtensions.TryParseHexStringRGBA(GameBase.Strings.GetValue("CharacterColor", DefaultCharacterColor.ToHexStringRGBA()),
			                                      out retColor);
			/*Color.TryParseHexString(GameBase.Strings.GetValue("CharacterColor", DefaultCharacterColor.ToHexStringRGBA()),
			                        out retColor);*/
			return (Color)retColor; }
	}
	public static bool CharacterColorChosen
	{
		get{ return GameBase.Strings.ContainsKey("CharacterColor"); }
	}
	public static Color DefaultCharacterColor
	{
		get{ return new Color(.4f, .20784314f, 0f); }
	}
	#endregion

	public enum GenderEnum
	{
		MALE, FEMALE
	}
}
