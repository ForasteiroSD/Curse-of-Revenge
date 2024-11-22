using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public AudioSource musica;
    public AudioSource sfx;
    
    [SerializeField]
    public AudioClip[] efeitos;
    [SerializeField]
    public AudioClip[] musicas;

    public void TocarSFX(int index)
    {
        print(efeitos[index].name);
        sfx.PlayOneShot(efeitos[index]);
    }


public void TrocarMusica(int indexMusica, float fadeDuration = 0.5f)
    {
        StartCoroutine(FadeOutIn(indexMusica, fadeDuration));
    }
    
    

    private IEnumerator FadeOutIn(int indexMusica, float duration)
    {
        if (musica.isPlaying)
        {
            // Fade Out
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                musica.volume = Mathf.Lerp(1, 0, t / duration);
                yield return null;
            }
            musica.Stop();
        }

        musica.clip = musicas[indexMusica];
        musica.Play();

        // Fade In
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musica.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
    }

}
