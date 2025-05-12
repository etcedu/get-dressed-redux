using UnityEngine;
using UnityEngine.SceneManagement;

//WTP TODO: This class doesn't do much right now but provides an easy place for us to change how all scenes are loaded
//I'm specifically thinking about this being able to handle showing a loading screen and performing async loading
public static class SceneLoader
{
    public static void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        SimpleRTVoiceExample.Instance?.PauseSpeech();

        if (CameraFade.Instance != null)
        {
            CameraFade.Instance.FadeOut( () => SceneManager.LoadScene(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}