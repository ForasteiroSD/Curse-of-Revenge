using System;
using System.Collections;
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
        yield return new WaitForSecondsRealtime(1.5f);
        _gameManager.SaveGame();
        StartCoroutine(_gameManager.LoadMenu("Credits"));
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
