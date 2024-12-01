using System.Collections;
using UnityEngine;
using Utils;

public class BossRoomEntry2 : MonoBehaviour
{
    public GameObject floorBlocker1; // O chão que desaparece
    public GameObject floorBlocker2; // O chão que desaparece
    public AudioManager audioManager;

    [SerializeField] GameObject _bossPrefab;
    [SerializeField] Transform _bossPosition;
    // [SerializeField] GameObject _bossHealthBar;

    Transform _inputManager; // Objeto pra pegar mensagens de input
    
    private bool _floorVisible = true; // Controle do estado do chão

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            AtivaFloorBlocker();
            // Remove o trigger para evitar reativação
            GetComponent<BoxCollider2D>().enabled = false;

            audioManager.TrocarMusica(7);

            // Spawn do boss e barra de vida
            StartCoroutine(SpawnBoss());
        }
    }

    private IEnumerator SpawnBoss()
    {
        yield return new WaitForSecondsRealtime(1);
        Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity, _inputManager);
        // _bossHealthBar.SetActive(true);
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _inputManager = GameObject.FindGameObjectWithTag("InputManager").transform;
    }

    public void AtivaFloorBlocker()
    {  
        // Ativa portas e toca sound effect das portas fechando
        audioManager.TocarSFX(8);
        floorBlocker1.SetActive(true);
        floorBlocker2.SetActive(true);
    }

    public void RemoveFloorBlocker()
    {
        // Desativa o chão permanentemente quando o boss for derrotado
        audioManager.TocarSFX(8);
        floorBlocker1.GetComponent<Animator>().SetTrigger("Open");
        floorBlocker2.GetComponent<Animator>().SetTrigger("Open");

        // Finaliza a música do boss
        audioManager.TrocarMusica(Random.Range(Constants.FIRST_SONG_INDEX, Constants.LAST_SONG_INDEX+1));

        // Destroy(_bossHealthBar);
    }
}
