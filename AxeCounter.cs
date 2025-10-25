using UnityEngine;
using TMPro; // Для TextMeshPro

public class AxeCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    private int choppedLogs = 0; // Счётчик

    public void IncrementCounter()
    {
        choppedLogs++;
        if (counterText != null)
        {
            counterText.text = $"{choppedLogs}";
        }
        Debug.Log($"Полено разрублено! Всего: {choppedLogs}");
    }
}
