using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsTracker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    public float avgFrameRate;
    public float CurrentFps;

    public void Update()
    {
        avgFrameRate = Time.frameCount / Time.time;
        CurrentFps = (int)(1f / Time.unscaledDeltaTime);
        tmp.SetText(CurrentFps.ToString());
    }
}
