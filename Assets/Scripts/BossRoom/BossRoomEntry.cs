using UnityEngine;

public class BossRoomEntry : MonoBehaviour
{
    public GameObject wallBlocker; // Arraste a barreira de bloqueio aqui pelo Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wallBlocker.SetActive(true); // Ativa a barreira de bloqueio
            Destroy(gameObject); // Remove o trigger para que ele não ative novamente
        }
    }

    public void RemoveWallBlocker()
    {
        // Desativa a barreira de bloqueio
        wallBlocker.SetActive(false);
    }
}