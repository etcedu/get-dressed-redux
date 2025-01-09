using System;

public static partial class EventRecorder
{
    //Game was started with selected company and position.
    [Serializable] public class LevelStartedEvent : GameEventData
    {
        public string levelAttemptId;
        public string company;
        public string position;
        public string levelName;
    }

    public static void RecordLevelStartedEvent(Guid levelAttemptId, string company, string position)
    {
        var e = new LevelStartedEvent()
        {
            levelAttemptId = levelAttemptId.ToString(),
            company = company,
            position = position,
            levelName = $"{company} - {position}",
        };
        GameEventManager.RecordEvent(e);
    }
    
    //WTP Note: Old name was "FinishedLevelEvent"
    [Serializable] public class LevelCompletedEvent : GameEventData
    {
        public string levelAttemptId;
        public string company;
        public string position;
        public string levelName;
        public bool passed;
        public int faceScore;
        public int topScore;
        public int bottomScore;
        public int shoeScore;
        public int otherScore;
        public int totalScore;
        public int passingThreshold;
        public float scorePercentage;
        public float duration;
    }

    public static void RecordLevelCompleted(Guid levelId, string company, string position, bool passed, int faceScore, int topScore, int bottomScore, int shoeScore, int otherScore, int totalScore, int passingThreshold, float duration)
    {
        float scorePercentage = (float)totalScore / passingThreshold;
        if (scorePercentage > 1)
            scorePercentage = 1;
        
        var e = new LevelCompletedEvent()
        {
            levelAttemptId = levelId.ToString(),
            company = company,
            position = position,
            levelName = $"{company} - {position}",
            passed = passed,
            faceScore = faceScore,
            topScore = topScore,
            bottomScore = bottomScore,
            shoeScore = shoeScore,
            otherScore = otherScore,
            totalScore = totalScore,
            passingThreshold = passingThreshold,
            scorePercentage = scorePercentage,
            duration = duration
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class RankChangedEvent : GameEventData
    {
        public string newRank;
    }

    public static void RecordRankChangedEvent(string newRank)
    {
        var e = new RankChangedEvent()
        {
            newRank = newRank,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class LevelRestartedEvent : GameEventData
    {
    }

    public static void RecordLevelRestarted()
    {
        var e = new LevelRestartedEvent();
        GameEventManager.RecordEvent(e);
    }

}