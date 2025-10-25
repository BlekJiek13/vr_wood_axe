using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class AxeCollisionHandler : MonoBehaviour
{
    public Collider bladeCollider;
    public float thresholdForce = 10f; // Пороговое значение скорости для раскалывания (в m/s)
    public float minImpactSpeed = 1f; // Минимальная скорость для активации коллизии (в m/s)
    public float maxDepth = 2.5f; // Максимальная глубина врезания топора в полено (в метрах)
    public GameObject splitLogPrefab; // Префаб расколотого полена
    public Transform axeBlade; // Трансформ лезвия топора
    public InputActionReference pullAxeAction; // Действие для выдергивания топора (Trigger)
    public TMP_Text debugText; 

    public AxeCounter axeCounter;
    private Rigidbody axeRb;
    private bool isStuck = false; // Флаг "застревания" топора

    private XRGrabInteractable grabInteractable; // Для проверки текущего контроллера
    private FixedJoint fixedJoint; // Для фиксации топора к полену

    void Start()
    {
        axeRb = GetComponentInParent<Rigidbody>(); // Получаем Rigidbody топора
        grabInteractable = GetComponentInParent<XRGrabInteractable>(); // Получаем XRGrabInteractable
        if (pullAxeAction != null)
        {
            pullAxeAction.action.performed += OnPullAxe;
        }
    }

    void Update()
    {
        // if (debugText != null)
        // {
        //     debugText.text = "Is Stuck: " + isStuck + "\nSpeed: " + (axeRb != null ? axeRb.linearVelocity.magnitude.ToString("F2") : "N/A") + " m/s";
        // }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].thisCollider == bladeCollider)
        {
            // Проверяем, удерживается ли топор
            if (!grabInteractable.isSelected)
            {
                if (debugText != null) //debugText.text += "\nТопор не удерживается, коллизия игнорируется";
                return;
            }

            if (collision.gameObject.CompareTag("Log") && !isStuck) // Проверяем тег и что топор не застрял
            {
                // Рассчитываем силу удара (скорость топора на момент контакта)
                float impactSpeed = collision.relativeVelocity.magnitude;

                if (debugText != null) debugText.text = "Impact Speed: " + impactSpeed.ToString("F2") + " m/s";

                // Игнорируем коллизии с низкой скоростью (случайный контакт)
                if (impactSpeed < minImpactSpeed)
                {
                    if (debugText != null) debugText.text += "\nСлишком слабый контакт, игнорирую";
                    return;
                }

                if (impactSpeed >= thresholdForce)
                {

                    
                    // Успешное раскалывание: уничтожаем исходное полено и спавним два расколотых
                    Destroy(collision.gameObject);
                    Vector3 splitPosition = collision.transform.position;
                    Quaternion splitRotation = collision.transform.rotation;

                    // Создаём первое полено с небольшим смещением влево
                    Vector3 offset1 = -transform.right * 0.5f; // Смещение вдоль оси топора
                    Instantiate(splitLogPrefab, splitPosition + offset1, splitRotation);

                    // Создаём второе полено с небольшим смещением вправо
                    Vector3 offset2 = transform.right * 0.5f; // Противоположное смещение
                    Instantiate(splitLogPrefab, splitPosition + offset2, splitRotation);

                    if (debugText != null) debugText.text += "\nПолено расколото на 2 части!";
                    if (axeCounter != null) axeCounter.IncrementCounter();
                }
                else
                {



                        //Рассчитываем глубину врезания
                        float depth = Mathf.Lerp(0.05f, maxDepth, (impactSpeed - minImpactSpeed) / (thresholdForce - minImpactSpeed));
                        depth = Mathf.Clamp(depth, 0.02f, maxDepth); // ограничиваем диапазон

                        //Берём точку контакта и нормаль поверхности
                        ContactPoint contact = collision.contacts[0];
                        Vector3 hitPoint = contact.point;
                        Vector3 hitNormal = contact.normal;

                        //Смещаем топор немного внутрь полена (вдоль лезвия)
                        Vector3 embedPosition = hitPoint - axeBlade.forward * depth;

                        axeRb.MovePosition(embedPosition);

                        //Создаём Joint, чтобы топор "держался" в дереве
                        fixedJoint = axeRb.gameObject.AddComponent<FixedJoint>();
                        fixedJoint.connectedBody = collision.rigidbody;
                        fixedJoint.breakForce = Mathf.Infinity;
                        fixedJoint.breakTorque = Mathf.Infinity;

                        isStuck = true;
                    

                        if (debugText != null)
                            debugText.text += $"\nТопор застрял в полене (глубина {depth:F2} м)";
                }
            }
        }
        
    }


    private void OnPullAxe(InputAction.CallbackContext context)
    {
        if (isStuck && grabInteractable.isSelected)
        {
            // Проверяем, что топор держится в руке
            var interactor = grabInteractable.interactorsSelecting[0];
            if (interactor != null)
            {
                if (debugText != null) debugText.text = "Trigger нажат, выдергиваю топор";

                // Уничтожаем FixedJoint для выдергивания
                if (fixedJoint != null)
                {
                    Destroy(fixedJoint);
                }

                axeRb.isKinematic = false;
                isStuck = false;
                if (debugText != null) debugText.text += "\nТопор выдернут!";
            }
        }
    }

    void OnDestroy()
    {
        // Отписываемся от действия выдергивания
        if (pullAxeAction != null)
        {
            pullAxeAction.action.performed -= OnPullAxe;
        }
    }
}

