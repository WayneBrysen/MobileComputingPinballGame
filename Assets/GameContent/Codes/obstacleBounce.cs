using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f;
    public int scoreValue;

    private GameManager gameManager; 
    private LeftFlipperControl leftFlipper;
    private RightFlipperControl rightFlipper;

    private GameObject hitSoundPrefab;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        leftFlipper = FindObjectOfType<LeftFlipperControl>();
        rightFlipper = FindObjectOfType<RightFlipperControl>();

        LoadCollisionAudioPrefab();
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            Vector3 normal = collision.contacts[0].normal;

            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            Debug.Log("Collided with object tagged: " + this.gameObject.tag);

            if (leftFlipper != null && leftFlipper.IsDoublePointsActive() ||
                rightFlipper != null && rightFlipper.IsDoublePointsActive())
            {
                scoreValue *= 2;
            }
            gameManager.AddScore(scoreValue);

            PlayCollisionSound(collision.contacts[0].point);

        }
    }

    void LoadCollisionAudioPrefab()
    {
        hitSoundPrefab = Resources.Load<GameObject>("hitSound");

        if (hitSoundPrefab != null)
        {
            Debug.Log("ObstacleBounce: Collision Audio Prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("ObstacleBounce: Failed to load Collision Audio Prefab. Check the path and ensure it is in a Resources folder.");
        }
    }

    void PlayCollisionSound(Vector3 collisionPoint)
    {
        if (hitSoundPrefab != null)
        {
            GameObject audioInstance = Instantiate(hitSoundPrefab, collisionPoint, Quaternion.identity);

            AudioSource audioSource = audioInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                StartCoroutine(DestroyAfterPlayback(audioInstance, audioSource));
            }
            else
            {
                Debug.LogError("ObstacleBounce: Collision Audio Prefab does not have an AudioSource component.");
                Destroy(audioInstance);
            }
        }
    }

    IEnumerator DestroyAfterPlayback(GameObject obj, AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        Destroy(obj);
    }


}