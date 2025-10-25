using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WoodPileSpawner : MonoBehaviour
{
public GameObject logPrefab; // Префаб бревна
    public Transform handPoint; // Точка спавна
    private XRSimpleInteractable interactable;
    private GameObject heldLog; // Текущее бревно в руке

    void Start()
    {
        // Получаем компонент XR Simple Interactable
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            // Подписываемся на событие выбора (например, нажатие Grip или Trigger)
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Проверяем, нет ли уже бревна в руке
        if (heldLog == null)
        {
            // Спавним бревно в точке руки
            heldLog = Instantiate(logPrefab, handPoint.position, handPoint.rotation);
            // Прикрепляем бревно к руке через XR Interaction Toolkit
            var grabInteractable = heldLog.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                // Подписываемся на событие отпускания бревна
                grabInteractable.selectExited.AddListener(OnSelectExited);
                // Автоматически "захватываем" бревно
                var interactor = args.interactorObject as IXRSelectInteractor;
                if (interactor != null)
                {
                    grabInteractable.interactionManager.SelectEnter(interactor, grabInteractable);
                }
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Очищаем heldLog, когда бревно отпущено
        heldLog = null;
    }

    void OnDestroy()
    {
        // Отписываемся от событий
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
        // Отписываемся от события отпускания, если бревно существует
        if (heldLog != null)
        {
            var grabInteractable = heldLog.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.RemoveListener(OnSelectExited);
            }
        }
    }
}
