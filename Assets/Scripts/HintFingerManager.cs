using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintFingerManager : MonoBehaviour
{
    public static HintFingerManager instance;
    List<HintFinger> hintFingers;

    Dictionary<string, float> timers = new();

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;
    }
    private void Start()
    {
        hintFingers = FindObjectsByType<HintFinger>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        HideAllFingers();
    }

    public static void ShowHintFinger(string fingerId)
    {
        instance.hintFingers.Find(x => x.id == fingerId)?.Show();
    }

    public static void HideHintFinger(string fingerId)
    {
        instance.hintFingers.Find(x => x.id == fingerId)?.Hide();
        CancelHintTimer(fingerId);
    }

    public static void HideAllFingers()
    {
        for (int i = 0; i < instance.hintFingers.Count; i++)
        {
            instance.hintFingers[i].Hide();
            CancelHintTimer(instance.hintFingers[i].id);
        }
    }

    public static void ShowHintOnTimer(string id, float delay)
    {
        if (!instance.timers.ContainsKey(id))
            instance.timers.Add(id, delay);
        else
            instance.timers[id] = delay;
    }

    public static void CancelHintTimer(string id)
    {
        instance.timers.Remove(id);
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
