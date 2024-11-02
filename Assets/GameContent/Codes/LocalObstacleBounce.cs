using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LocalObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f;

    private Volume volume;
    private Bloom bloomEffect;

    private GameObject hitSound;

    void Awake()
    {
        LoadCollisionAudioPrefab();
    }

    void Start()
    {
        volume = FindObjectOfType<Volume>();

        if (volume != null)
        {
            if (volume.profile.TryGet(out bloomEffect))
            {
                Debug.Log("Bloom effect found and ready to use.");
            }
            else
            {
                Debug.LogWarning("Bloom effect not found in the Volume profile.");
            }
        }
        else
        {
            Debug.LogError("Volume component not found in the scene.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            Vector3 normal = collision.contacts[0].normal;
            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            if (collision.collider.CompareTag("BlueBall"))
            {
                ChangeBloomTint(HexToColor("4E5EFF"));
            }
            else if (collision.collider.CompareTag("RedBall"))
            {
                ChangeBloomTint(HexToColor("E25AFF"));
            }

            PlayCollisionSound();
        }
    }

    void ChangeBloomTint(Color tintColor)
    {
        if (bloomEffect != null)
        {
            bloomEffect.tint.value = tintColor;
        }
    }

    void LoadCollisionAudioPrefab()
    {

        hitSound = Resources.Load<GameObject>("hitSound");

        if (hitSound != null)
        {
            Debug.Log("Collision Audio Prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load Collision Audio Prefab. Check the path and ensure it is in a Resources folder.");
        }
    }

    void PlayCollisionSound()
    {
        if (hitSound != null)
        {
            GameObject audioInstance = Instantiate(hitSound, transform.position, Quaternion.identity);

            AudioSource audioSource = audioInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                StartCoroutine(DestroyAfterPlayback(audioInstance, audioSource));
            }
            else
            {
                Debug.LogError("Collision Audio Prefab does not have an AudioSource component.");
                Destroy(audioInstance);
            }
        }
    }

    IEnumerator DestroyAfterPlayback(GameObject obj, AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        Destroy(obj);
    }


    Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString($"#{hex}", out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex color format.");
            return Color.white;
        }
    }
}