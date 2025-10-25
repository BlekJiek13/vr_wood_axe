using UnityEngine;
using TMPro; // Для TextMeshPro

public class AxeCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText; // Ссылка на UI-текст (задавай в инспекторе)
    private int choppedLogs = 0; // Счётчик

    // Вызывай это в OnCollisionEnter, когда полено расколото (в if (impactSpeed >= thresholdForce))
    public void IncrementCounter()
    {
        choppedLogs++;
        if (counterText != null)
        {
            counterText.text = $"{choppedLogs}";
        }
        Debug.Log($"Полено разрублено! Всего: {choppedLogs}");
    }

    // В AxeCollisionHandler, в блоке раскалывания:
    // IncrementCounter(); // Добавь эту строку после Instantiate(splitLogPrefab...)
}