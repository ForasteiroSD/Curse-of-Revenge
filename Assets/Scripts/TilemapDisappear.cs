using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDisappear : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float disappearTime = 2f; // Tempo que o chão ficará desaparecido
    [SerializeField] private float reappearTime = 3f; // Tempo até o chão reaparecer

    private Tilemap tilemap; // Referência ao Tilemap

    void Start()
    {
        // Obter o Tilemap do objeto
        tilemap = GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Tilemap não encontrado! Certifique-se de que o script está no objeto Tilemap.");
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se o objeto que entrou no Trigger é o Player
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ativou o desaparecimento.");
            StartCoroutine(DisappearAndReappear());
        }
    }

    private System.Collections.IEnumerator DisappearAndReappear()
    {
        // Desativa o Tilemap (fazendo o chão desaparecer)
        tilemap.gameObject.SetActive(false);
        Debug.Log("Chão desapareceu.");

        // Espera o tempo de desaparecimento
        yield return new WaitForSeconds(disappearTime);

        // Reativa o Tilemap após o tempo definido
        yield return new WaitForSeconds(reappearTime);
        tilemap.gameObject.SetActive(true);
        Debug.Log("Chão reapareceu.");
    }
}