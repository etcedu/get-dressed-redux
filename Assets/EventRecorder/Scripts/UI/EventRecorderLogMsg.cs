using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderLogMsg : MonoBehaviour
    {
        [SerializeField] Text timestampUI;
        [SerializeField] Text msgTypeUI;
        [SerializeField] Text msgPreviewUI;

        public string Msg { get; private set; }
        public string PreviewMsg { get; private set; }
        public LogType Type { get; private set; }
        public string Timestamp { get; private set; }

        public void UpdateData(EventRecorderLogMsg eventRecorderLogMsg)
        {
            UpdateData(eventRecorderLogMsg.Timestamp, eventRecorderLogMsg.Type, eventRecorderLogMsg.Msg);
        }

        public void UpdateData(string timeStamp, LogType type, string msg)
        {
            Type = type;
            Msg = msg;
            Timestamp = timeStamp;

            Msg = msg.Replace('\n', ' ');
            if (Msg.Length > EventRecorderSettingsSO.MessagePreviewLength)
            {
                PreviewMsg = Msg.Substring(0, EventRecorderSettingsSO.MessagePreviewLength);
                PreviewMsg += "...";
            }
            else
            {
                PreviewMsg = Msg;
            }

            RefreshUI();
            gameObject.SetActive(true);
        }

        public void RefreshUI()
        {
            string color;
            switch (Type)
            {
                case LogType.Log:
                    color = string.Format("<color={0}>", EventRecorderSettingsSO.LogColor);
                    break;
                case LogType.Warning:
                    color = string.Format("<color={0}>", EventRecorderSettingsSO.WarningColor);
                    break;
                case LogType.Error:
                    color = string.Format("<color={0}>", EventRecorderSettingsSO.ErrorColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }

            timestampUI.text = string.Format("[{0}]", Timestamp);
            msgTypeUI.text = string.Format("{0}[{1}]</color>", color, Type.ToString());
            msgPreviewUI.text = string.Format("{0}", PreviewMsg);
        }

        public void OnPressed()
        {
            EventRecorderLog.ShowEventInspector(this);
        }
    }
}