using UnityEngine;

public class AppLogger : IAppLogger
{
    public void Log(string message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(message);
#endif
    }

    public void LogWarning(string message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning(message);
#endif
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
}