using System;
using UnityEngine;
public class CustomLogger
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void Log(object obj)
    {
#if UNITY_EDITOR
        Debug.Log(obj.ToString());
#endif
    }

    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }

    public static void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }

    public static void LogException(Exception exception)
    {
#if UNITY_EDITOR
        Debug.LogException(exception);
#endif
    }

    public static void LogException(string exception)
    {
        LogError(exception);
    }

    public static void LogStackTrace(Exception exception)
    {
        LogError(exception.StackTrace);
    }

    public static void LogExceptionMessage(Exception exception)
    {
        LogError(exception.Message);
    }
}
