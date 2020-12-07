using UnityEngine;

public class LogScreen : MonoBehaviour
{
    string logText = "";
    static readonly int logLength = 300;

    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        // truncate log to tail
        if (logText.Length > logLength)
        {
            logText = logText.Substring(logText.Length - logLength, logLength);
        }

        logText += "log: " + logString;
        if (type != LogType.Log)
            logText += "\nstack: " + stackTrace;
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), logText);
    }
}
