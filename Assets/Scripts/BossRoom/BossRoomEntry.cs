using UnityEngine;

public class BossRoomEntry : MonoBehaviour
{
    public GameObject wallBlocker; // Arraste a barreira de bloqueio aqui pelo Inspector
    public AudioManager audioManager;
    public AudioClip bossMusic;
    public AudioClip clip;
    [SerializeField] GameObject _bossPrefab;
    [SerializeField] Transform _bossPosition;

    Animator _animWallBlocker;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animWallBlocker = wallBlocker.GetComponent<Animator>();
            _animWallBlocker.SetTrigger("Close"); //ativa barreira
            if (bossMusic != null)
                audioManager.TrocarMusica(bossMusic, 1f);
            GetComponent<BoxCollider2D>().enabled = false; // Remove o trigger para que ele não ative novamente
            Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity);
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
        audioManager.TrocarMusica(clip);

        Destroy(gameObject, 1f);
    }
}