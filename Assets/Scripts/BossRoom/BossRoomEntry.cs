using UnityEngine;
using Utils;

public class BossRoomEntry : MonoBehaviour
{
    public GameObject wallBlocker; // Arraste a barreira de bloqueio aqui pelo Inspector
    public AudioManager audioManager;
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
            audioManager.TrocarMusica(Constants.NB_SONG_INDEX);
            GetComponent<BoxCollider2D>().enabled = false; // Remove o trigger para que ele não ative novamente
            Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity);
            _bossHealthBar.SetActive(true);
        }
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void RemoveWallBlocker()
    {
        // Desativa a barreira de bloqueio
        _animWallBlocker.SetTrigger("Open");

        // Desativa música do boss
        audioManager.TrocarMusica(Random.Range(Constants.FIRST_SONG_INDEX, Constants.LAST_SONG_INDEX+1));

        Destroy(gameObject, 1f);
        Destroy(_bossHealthBar);
    }
}