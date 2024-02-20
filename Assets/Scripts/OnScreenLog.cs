using TMPro;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;

    private void Awake()
    {
        logText.SetText(string.Empty);
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnLogMessage(string message, string stacktrace, LogType type)
    {
        logText.text += ($"[{type}] {message}\n\n");
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessage;
    }
}
