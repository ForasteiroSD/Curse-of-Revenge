using UnityEngine;

public class BossRoomEntry : MonoBehaviour
{
    public GameObject wallBlocker; // Arraste a barreira de bloqueio aqui pelo Inspector
    public AudioManager audioManager;
    public AudioClip bossMusic;
    public AudioClip clip;
    [SerializeField] GameObject _bossPrefab;
    [SerializeField] Transform _bossPosition;
    [SerializeField] GameObject _bossHealthBar;

    Animator _animWallBlocker;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animWallBlocker = wallBlocker.GetComponent<Animator>();
            audioManager.TocarSFX(8);
            _animWallBlocker.SetTrigger("Close"); //ativa barreira
            if (bossMusic != null)
                audioManager.TrocarMusica(1, 1f);
            GetComponent<BoxCollider2D>().enabled = false; // Remove o trigger para que ele não ative novamente
            Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity);
            _bossHealthBar.SetActive(true);
        }
    }

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void RemoveWallBlocker()
    {
        // Desativa a barreira de bloqueio
        _animWallBlocker.SetTrigger("Open");

        // Desativa música do boss
        audioManager.TrocarMusica(0);

        Destroy(gameObject, 1f);
        Destroy(_bossHealthBar);
    }
}