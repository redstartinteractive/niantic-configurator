using TMPro;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;

    private void Awake()
    {
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnLogMessage(string condition, string stacktrace, LogType type)
    {
        logText.text += $"[{type}] {condition}\n\n";
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessage;
    }
}
