using UnityEngine;

public class BossRoomEntry2 : MonoBehaviour
{
    public GameObject floorBlocker; // O chão que desaparece
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

            // Spawn do boss e barra de vida
            Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity, _inputManager);
            // _bossHealthBar.SetActive(true);
        }
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _inputManager = GameObject.FindGameObjectWithTag("InputManager").transform;
    }

    public void AtivaFloorBlocker()
    {
        floorBlocker.SetActive(true);
    }

    public void RemoveFloorBlocker()
    {
        // Desativa o chão permanentemente quando o boss for derrotado
        floorBlocker.SetActive(false);

        // Finaliza a música do boss
        audioManager.TrocarMusica(0);

        // Destroy(_bossHealthBar);
    }
}
