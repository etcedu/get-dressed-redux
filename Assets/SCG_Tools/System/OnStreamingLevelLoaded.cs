using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnStreamingLevelLoaded : MonoBehaviour
{
    [SerializeField]
    private List<string> levels;
    [SerializeField]
    private List<EventDelegate> events;

    IEnumerator Start()
    {
        List<string> _levels = new List<string>(levels);

        while (_levels.Count > 0)
        {
            if (Application.CanStreamedLevelBeLoaded(_levels[0]))
                _levels.RemoveAt(0);
            yield return null;
        }

        EventDelegate.Execute(events);
    }
}
