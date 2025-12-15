using System;
using master;
using UnityEngine;
using UnityEngine.Profiling;

public class FPSDisplay : SingletonDDOL<FPSDisplay>
{
    private float deltaTime = 0.0f;
    [SerializeField] private bool IsDeveloper;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (!IsDeveloper) return;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!IsDeveloper) return;

        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = h * 2 / 100,
            normal = { textColor = Color.red }
        };

        // FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // RAM
        float ramUsed = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        float ramReserved = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);

        string text =
            $"{msec:0.0} ms ({fps:0.} fps)\n" +
            $"RAM Used: {ramUsed:0.0} MB\n" +
            $"RAM Reserved: {ramReserved:0.0} MB";

        Rect rect = new Rect(10, 10, w, h * 6 / 100);
        GUI.Label(rect, text, style);
    }
}
