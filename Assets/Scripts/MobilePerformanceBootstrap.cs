using UnityEngine;

public static class MobilePerformanceBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configure()
    {
        QualitySettings.vSyncCount = 0;

        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }
}
