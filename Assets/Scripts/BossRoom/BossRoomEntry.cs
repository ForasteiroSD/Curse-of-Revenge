using System;
using UnityEngine;

public class BossRoomEntry : MonoBehaviour
{
    public GameObject wallBlocker; // Arraste a barreira de bloqueio aqui pelo Inspector
    public AudioManager audioManager;
    public AudioClip bossMusic;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wallBlocker.SetActive(true); // Ativa a barreira de bloqueio
            if (bossMusic != null)
                audioManager.TrocarMusica(bossMusic, 1f);
            Destroy(gameObject); // Remove o trigger para que ele não ative novamente
        }
    }

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void RemoveWallBlocker()
    {
        // Desativa a barreira de bloqueio
        wallBlocker.SetActive(false);
    }
}