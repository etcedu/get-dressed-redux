using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    [CreateAssetMenu(menuName = "Event Recorder/Endpoint", fileName = "New Endpoint", order = 0)]
    public class EventRecorderEndpointSO : ScriptableObject
    {
        public string endpoint;
        public string apiKey;
    }
}