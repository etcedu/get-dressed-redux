using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/* Criteria:
 *  - When called a parental gate resource will be loaded and instantiated.
 *     - Canvas will be self contained in the prefab.
 *  - Will show a message explaining to the user a parental gate must be passed
 *  - Systems can request a parental gate. Will pass in a callback method that will return if the gate was passed or if the user stopped trying.
 *  - Will choose randomly from a selection of parental gates
 *  - If a gate is failed a new one will be selected
 *  - The callback will only be called with a "failed" report if the user quits the gate
 * 
 */

namespace SimcoachGames.EventRecorder
{
    public class ParentalGateManager : MonoBehaviour
    {
        const string ParentalGatePrefabName = "ParentalGateManager";

        static ParentalGateManager CurrentInstance;
        static ParentalGateChallengeBase CurrentChallenge;
        static Action<bool> CurrentGateCompleteCallback;
        
        [SerializeField] List<ParentalGateChallengeBase> challenges;
        [SerializeField] Transform challengeParent;
        [SerializeField] AudioClip askYourParentsAudioClip;

        static void SpawnParentalGateInstance()
        {
            var manager = Resources.Load<ParentalGateManager>(ParentalGatePrefabName);
            CurrentInstance = Instantiate(manager);
            CurrentInstance.ShowNewChallenge();
        }

        static void DestroyParentalGateInstance()
        {
            Destroy(CurrentInstance.gameObject);
        }

        void ShowNewChallenge()
        {
            ParentalGateChallengeBase newChallenge = challenges.GetRandom();

            if (CurrentChallenge != null)
                Destroy(CurrentChallenge.gameObject);

            CurrentChallenge = Instantiate(newChallenge, challengeParent);
            CurrentChallenge.Init(OnChallengeSucceededCallback, OnChallengeFailedCallback);
        }

        public static void RequestParentalGate(Action<bool> gateCompleteCallback)
        {
            // EventRecorderLog.Log("Showing Parental Gate...");
            //If there is already an active gate - report it failed and kill it
            if (CurrentInstance != null)
            {
                CurrentGateCompleteCallback?.Invoke(false);
                DestroyParentalGateInstance();
            }

            CurrentGateCompleteCallback = gateCompleteCallback;
            SpawnParentalGateInstance();
        }

        void OnChallengeSucceededCallback()
        {
            // EventRecorderLog.Log("Parental Gate Passed");
            CurrentGateCompleteCallback?.Invoke(true);
            DestroyParentalGateInstance();
        }

        void OnChallengeFailedCallback()
        {
            // EventRecorderLog.Log("Parental Gate Failed... showing new challenge...");
            ShowNewChallenge();
        }

        public void CancelPressed()
        {
            // EventRecorderLog.Log("Parental Gate Cancelled");
            CurrentGateCompleteCallback?.Invoke(false);
            DestroyParentalGateInstance();
        }

        public void AskParentsAudioPressed()
        {
            GetComponent<AudioSource>().PlayOneShot(askYourParentsAudioClip);
        }


        #region Editor
#if UNITY_EDITOR
        [ContextMenu("Get Challenge")]
        public void Editor_GetChallenge()
        {
            ShowNewChallenge();
        }
#endif
        #endregion
        
    }
}
