using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SimcoachGames.EventRecorder
{
    /* WTP NOTE: Since we always generate a session ID and Device ID I am removing the ID display. If people want it
     * back I will re-activate it.
     *
     * Deciding when UI shows up depending on a scene is being moved to the GameEventManager
     */
    public class EventRecorderInfoDisplayUI : MonoBehaviour
    {
        // [Header("Game Specific Settings")]
        // [Tooltip("Assuming the User ID mode is set to 'Session' the ability to generate a new session ID will be available in the initial scene that includes the EventRecordingSystem prefab as well as the listed scenes.")]
        // [SerializeField]
        // List<string> scenesSessionIdCanBeGenerated;

        [Header("Don't Touch!")] 
        [SerializeField] Text infoDisplayUI;

        // [SerializeField] float pressTime;
        // [SerializeField] Text currentIdUI;
        // [SerializeField] GameObject generateNewSessionIdButton;
        [SerializeField] GameObject openEventLogButton;

        void Awake()
        {
            if (EventRecorderSettingsSO.GetInstance().buildUseCase == BuildUseCase.AppStore)
                gameObject.SetActive(false);
        }

        void Start()
        {
            // scenesSessionIdCanBeGenerated.Add(SceneManager.GetActiveScene().name);
            // SceneManager.sceneLoaded += (scene, mode) => UpdateSessionIdDisplay();
            // UpdateSessionIdDisplay();
            HideFullInfo();
        }

        float _timeSincePress;
        int _timesPressed;
        bool _infoShown;
        void Update()
        {
            if (_infoShown)
                return;

            _timeSincePress += Time.unscaledDeltaTime;
            if (_timeSincePress > 2)
                _timesPressed = 0;
        }


        public void OnButtonPressed()
        {
            print("Pressed");
            if (_infoShown)
            {
                HideFullInfo();
            }
            else
            {
                _timesPressed++;
                _timeSincePress = 0;
                if (_timesPressed >= 5)
                    ShowFullInfo();
            }
        }

        // public void OnGenNewSessionIdPressed()
        // {
        //     EventRecorderId.GenerateNewSessionId();
        //     UpdateSessionIdDisplay();
        // }

        // void UpdateSessionIdDisplay()
        // {
        //     currentIdUI.text = $"[UserId] {EventRecorderId.UserId.ToString()}";
        //     generateNewSessionIdButton.SetActive(scenesSessionIdCanBeGenerated.Contains(SceneManager.GetActiveScene().name));
        // }

        void ShowFullInfo()
        {
            infoDisplayUI.text = EventRecorderSettingsSO.GetInfoStringWithNewlines();
            openEventLogButton.SetActive(EventRecorderSettingsSO.GetInstance().showRuntimeLog);
            _infoShown = true;
        }

        void HideFullInfo()
        {
            infoDisplayUI.text = string.Format("v{0}", Application.version);
            openEventLogButton.SetActive(false);
            _timesPressed = 0;
            _infoShown = false;
        }

        public void ToggleRuntimeLog()
        {
            if (!EventRecorderSettingsSO.GetInstance().showRuntimeLog)
                return;

            EventRecorderLog.ToggleRuntimeLog();
        }


    }
}