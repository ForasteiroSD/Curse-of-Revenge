using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDisappear : MonoBehaviour
{

    private GameObject tilemap; // Referência ao Tilemap

    void Start()
    {
        // Obter o Tilemap do objeto
        tilemap = GameObject.FindGameObjectWithTag("disappear");

       
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se o objeto que entrou no Trigger é o Player
        if (collider.gameObject.CompareTag("Player"))
        {
            tilemap.SetActive(false);
        }
    }

   
}