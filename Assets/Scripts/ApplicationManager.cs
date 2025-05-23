using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static void FinishBodyLanguageTraining()
    {
        PlayerPrefs.SetInt("FinishedBodyLanguageTraining", 1);
    }

    public static void FinishQuestionTraining()
    {
        Debug.Log("----- Finish question training()");
        PlayerPrefs.SetInt("FinishedQuestionsTraining", 1);
    }

    public static bool HasFinishedBodyLanguageTraining()
    {
        return PlayerPrefs.GetInt("FinishedBodyLanguageTraining", 0) == 1;
    }

    public static bool HasFinishedQuestionsTraining()
    {
        return PlayerPrefs.GetInt("FinishedQuestionsTraining", 0) == 1;
    }

    public static void ResetData()
    {
        PlayerPrefs.SetInt("FinishedBodyLanguageTraining", 0);
        PlayerPrefs.SetInt("FinishedQuestionsTraining", 0);
        PlayerPrefs.SetInt($"hired_stocker", 0);
        PlayerPrefs.SetInt($"hired_bakery", 0);
        PlayerPrefs.SetInt($"hired_cashier", 0);
        PlayerPrefs.SetInt($"hired_lot", 0);
    }

    public static void UnlockGame()
    {
        PlayerPrefs.SetInt("FinishedBodyLanguageTraining", 1);
        PlayerPrefs.SetInt("FinishedQuestionsTraining", 1);
    }
}
