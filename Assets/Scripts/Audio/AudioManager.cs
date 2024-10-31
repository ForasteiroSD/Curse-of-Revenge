using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void TrocarMusica(AudioClip clip, float fadeDuration = 0.5f)
    {
        StartCoroutine(FadeOutIn(clip, fadeDuration));
    }

    private IEnumerator FadeOutIn(AudioClip newClip, float duration)
    {
        if (audioSource.isPlaying)
        {
            // Fade Out
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(1, 0, t / duration);
                yield return null;
            }
            audioSource.Stop();
        }

        audioSource.clip = newClip;
        audioSource.Play();

        // Fade In
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
    }

}
