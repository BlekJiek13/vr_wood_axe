using UnityEngine;

public class SoundLog : MonoBehaviour
{

    public AudioClip destroyClip;
    private Collider bladeCollider;

    private void Start()
    {
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
                audioSource.spatialBlend = 1f;
                audioSource.volume = 1f;
                audioSource.transform.position = transform.position;

                audioSource.Play();

                // уничтожаем временный объект после проигрывания
                Destroy(tempAudio, destroyClip.length);
            }
        }
    }
}
