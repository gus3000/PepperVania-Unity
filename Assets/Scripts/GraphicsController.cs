using System;
using UnityEngine;

public class GraphicsController : MonoBehaviour
{
    [Range(0,144)] public int targetFramerate;
    private void Start()
    {
        Application.targetFrameRate = targetFramerate;
        // QualitySettings.vSyncCount = 1;
    }
}