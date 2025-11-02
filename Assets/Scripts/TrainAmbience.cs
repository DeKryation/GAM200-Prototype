using System.Collections;
using UnityEngine;

public class TrainSFXLooper : MonoBehaviour
{
    [Header("Train Sound Settings")]
    [SerializeField] private AudioClip trainClip;   // drag your audio here
    [SerializeField] private float intervalSeconds = 20f; // delay between plays
    [Range(0f, 1f)][SerializeField] private float volume = 1f;

    private Coroutine loopCoroutine;
    private AudioSource audioSource;

    private void Awake()
    {
        // Create or reuse an AudioSource for this looper
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    private void OnEnable()
    {
        loopCoroutine = StartCoroutine(PlayTrainSFXLoop());
    }

    private void OnDisable()
    {
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);
    }

    private IEnumerator PlayTrainSFXLoop()
    {
        while (true)
        {
            if (trainClip != null)
            {
                audioSource.PlayOneShot(trainClip, volume);
            }
            else
            {
                Debug.LogWarning("No TrainSFX AudioClip assigned to TrainSFXLooper!");
            }

            yield return new WaitForSeconds(intervalSeconds);
        }
    }
}