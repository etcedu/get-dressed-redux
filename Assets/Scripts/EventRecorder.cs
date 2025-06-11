using UnityEngine;
using System.Collections;
using System;

public static partial class EventRecorder
{
    [Serializable] public class CompletedTutorialEvent : GameEventData
    {
        public float duration;
    }

    public static void RecordCompletedTutorialEvent(float duration)
    {
        var e = new CompletedTutorialEvent()
        {
            duration = duration,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class LevelStartedEvent : GameEventData
    {
        public string levelName;
    }

    public static void RecordLevelStartedEvent(string levelName)
    {
        var e = new LevelStartedEvent()
        {
            levelName = levelName,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class LevelCompletedEvent : GameEventData
    {
        public float duration;
        public bool passed;
        public float score;
        public string levelname;
        public string headPiece;
        public int headScore;
        public string topPiece;
        public int topScore;
        public string bottomPiece;
        public int bottomScore;
        public string feetPiece;
        public int feetScore;
    }

    public static void RecordLevelCompletedEvent(float duration, bool passed, float score, string levelname, string headPiece, int headScore, string topPiece, int topScore, string bottomPiece, int bottomScore, string feetPiece, int feetScore)
    {
        var e = new LevelCompletedEvent()
        {
            duration = duration,
            passed = passed,
            score = score,
            levelname = levelname,
            headPiece = headPiece,
            headScore = headScore,
            topPiece = topPiece,
            topScore = topScore,
            bottomPiece = bottomPiece,
            bottomScore = bottomScore,
            feetPiece = feetPiece,
            feetScore = feetScore,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class LevelRestartedEvent : GameEventData
    {
        public float duration;
        public string levelname;
    }

    public static void RecordLevelRestartedEvent(float duration, string levelname)
    {
        var e = new LevelRestartedEvent()
        {
            duration = duration,
            levelname = levelname,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class LevelQuitEvent : GameEventData
    {
        public float duration;
        public string levelname;
    }

    public static void RecordLevelQuitEvent(float duration, string levelname)
    {
        var e = new LevelQuitEvent()
        {
            duration = duration,
            levelname = levelname,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class ManuallyPlayedVoiceOverEvent : GameEventData
    {
        public string speakerId;
        public string messagePreview;
    }

    public static void RecordManuallyPlayedVoiceOverEvent(string speakerId, string messagePreview)
    {
        var e = new ManuallyPlayedVoiceOverEvent()
        {
            speakerId = speakerId,
            messagePreview = messagePreview,
        };
        GameEventManager.RecordEvent(e);
    }

    [Serializable] public class ViewedDetailedFeedbackEvent : GameEventData
    {
    }

    public static void RecordViewedDetailedFeedbackEvent()
    {
        var e = new ViewedDetailedFeedbackEvent()
        {
        };
        GameEventManager.RecordEvent(e);
    }

}