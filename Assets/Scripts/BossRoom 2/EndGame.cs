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
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(_gameManager.LoadMenu("Credits"));
    }


    private IEnumerator FadeOutSounds(float duration) {
        GameObject audioManager = GameObject.Find("AudioManager");
        if(audioManager != null) {
            AudioSource[] audioSources = audioManager.GetComponents<AudioSource>();

            for (float t = 0; t < duration; t += Time.deltaTime) {
                foreach (var source in audioSources) {
                    source.volume = Mathf.Lerp(1, 0, t / duration);
                    yield return null;
                }
            }
        }
    }
}
