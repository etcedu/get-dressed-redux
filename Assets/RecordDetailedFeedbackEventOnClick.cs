using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordDetailedFeedbackEventOnClick : MonoBehaviour
{
    public void SendEvent()
    {
        EventRecorder.RecordViewedDetailedFeedbackEvent();
    }
}
