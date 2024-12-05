using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            StartCoroutine(TrocaLevel());
        }
    }
    
    private IEnumerator TrocaLevel()
    {
        GameManager _gameManager = FindFirstObjectByType<GameManager>();
        _gameManager._level = 1;
        _gameManager.SaveGame();
        FindFirstObjectByType<Adventurer>().SetEndGame();
        StartCoroutine(FadeOutSounds(2f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(_gameManager.LoadMenu("Credits"));
    }


    private IEnumerator FadeOutSounds(float duration) {
        GameObject audioManager = GameObject.Find("AudioManager");
        if(audioManager != null) {
            AudioSource[] audioSources = audioManager.GetComponents<AudioSource>();
            float[] startVolume = new float[audioSources.Length];

            for(int i = 0; i < audioSources.Length; i++) startVolume[i] = audioSources[i].volume;

            for (float t = 0; t < duration; t += Time.deltaTime) {
                for(int i = 0; i < audioSources.Length; i++) {
                    audioSources[i].volume = Mathf.Lerp(startVolume[i], 0, t / duration);
                    yield return null;
                }
            }
        }
    }
}
