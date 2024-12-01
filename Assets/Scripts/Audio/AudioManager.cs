using System.Collections;
using UnityEngine;
using Utils;

public class AudioManager : MonoBehaviour
{
    public AudioSource musica;
    public AudioSource sfx;

    [SerializeField]
    public AudioClip[] efeitos;
    [SerializeField]
    public AudioClip[] musicas;

    private int musicaAtual;

    private bool trocandoMusica = false;

    public void TocarSFX(int index)
    {
        if (efeitos.Length - 1 >= index)
        {
            sfx.PlayOneShot(efeitos[index]);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        TrocarMusica(Random.Range(Constants.FIRST_SONG_INDEX, Constants.LAST_SONG_INDEX+1));
    }

    private void Update()
    {
        // Verifica se a música terminou e ainda não está trocando
        if (!musica.isPlaying && !trocandoMusica)
        {
            trocandoMusica = true; 
            int novaMusica = Random.Range(Constants.FIRST_SONG_INDEX, Constants.LAST_SONG_INDEX+1);
            while (novaMusica == musicaAtual)
            {
                novaMusica = Random.Range(Constants.FIRST_SONG_INDEX, Constants.LAST_SONG_INDEX+1);
            }
            musicaAtual = novaMusica;
            TrocarMusica(musicaAtual);
        }
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

        trocandoMusica = false; // Libera para a próxima troca
    }
}
