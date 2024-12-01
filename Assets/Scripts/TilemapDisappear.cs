using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDisappear : MonoBehaviour
{
    [SerializeField] GameObject tilemap; // Referência ao Tilemap

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se o objeto que entrou no Trigger é o Player
        if (collider.gameObject.CompareTag("Player"))
        {
            tilemap.GetComponent<Animator>().SetTrigger("Disappear");
            GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().TocarSFX(25);
            Destroy(gameObject);
        }
    }

}