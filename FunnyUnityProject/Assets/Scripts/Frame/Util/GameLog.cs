using UnityEngine;

namespace GFrame
{
    public static class GameLog
    {
        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        public static void LogWarn(string msg)
        {
            Debug.LogWarning(msg);
        }

        public static void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        public static void Assert(bool condition, string msg)
        {
            Debug.Assert(condition, msg);
        }
    }
}