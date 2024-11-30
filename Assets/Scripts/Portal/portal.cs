using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Nome exato da cena para onde o portal leva

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        // Verifica se o jogador entrou no portal
        if (other.CompareTag("Player")) // Verifique se o jogador possui a tag "Player"
        
        {
            print("portalizou");
            // Carrega a próxima cena usando o nome da cena
            SceneManager.LoadScene(nextSceneName);
        }
    }
}