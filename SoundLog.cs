using UnityEngine;

public class SoundLog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    public AudioClip destroyClip;
    private Collider bladeCollider;

    private void Start()
    {
        // Находим коллайдер топора по тегу
        GameObject blade = GameObject.FindGameObjectWithTag("AxeBlade");
        if (blade != null)
        {
            bladeCollider = blade.GetComponent<Collider>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Ударился: {collision.collider.name}, ожидался: {bladeCollider.name}");

        if (collision.collider == bladeCollider)
        {
            
            // Создаём временный объект для звука
            if (destroyClip != null)
            {
                GameObject tempAudio = new GameObject("TempSound");
                AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

                audioSource.clip = destroyClip;
                audioSource.spatialBlend = 1f; // 3D-звук
                audioSource.volume = 1f;
                audioSource.transform.position = transform.position;

                audioSource.Play();

                // уничтожаем временный объект после проигрывания
                Destroy(tempAudio, destroyClip.length);
            }
        }
    }
}
