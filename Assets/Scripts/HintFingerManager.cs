using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintFingerManager : MonoBehaviour
{
    [SerializeField] List<HintFinger> hintFingers;
    Dictionary<string, float> timers = new();
    
    void Start()
    {
        FindHintFingers();
        HideAllFingers();
    }

    void FindHintFingers()
    {
        hintFingers = FindObjectsByType<HintFinger>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    public void ShowHintFinger(string fingerId)
    {
        Debug.Log($"show {fingerId}");

        hintFingers.Find(x => x.id == fingerId)?.Show();
    }

    public void HideHintFinger(string fingerId)
    {
        Debug.Log($"hide {fingerId}");

        hintFingers.Find(x => x.id == fingerId)?.Hide();
        CancelHintTimer(fingerId);
    }

    public void HideAllFingers()
    {
        FindHintFingers();

        Debug.Log($"hide all {hintFingers.Count}");
        for (int i = 0; i < hintFingers.Count; i++)
        {
            hintFingers[i].Hide();
            CancelHintTimer(hintFingers[i].id);
        }
    }

    public void ShowHintOnTimer(string id, float delay)
    {
        if (!timers.ContainsKey(id))
            timers.Add(id, delay);
        else
            timers[id] = delay;
    }

    public void CancelHintTimer(string id)
    {
        timers.Remove(id);
    }

    private void Update()
    {
        for (int i = timers.Count-1; i >= 0; i--)
        {
            timers[timers.Keys.ElementAt(i)] -= Time.deltaTime;

            if (timers[timers.Keys.ElementAt(i)] <= 0f)
            {
                ShowHintFinger(timers.Keys.ElementAt(i));
                timers.Remove(timers.Keys.ElementAt(i));
            }
        }
    }
}
