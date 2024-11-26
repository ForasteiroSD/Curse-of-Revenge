using UnityEngine;

public class BossRoomEntry2 : MonoBehaviour
{
    public GameObject floorBlocker; // O chão que desaparece
    public AudioManager audioManager;
    public AudioClip bossMusic;
    [SerializeField] GameObject _bossPrefab;
    [SerializeField] Transform _bossPosition;
    [SerializeField] GameObject _bossHealthBar;

    Animator _animFloorBlocker;
    private bool _floorVisible = true; // Controle do estado do chão

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animFloorBlocker = floorBlocker.GetComponent<Animator>();

            // Faz o chão desaparecer temporariamente
            StartCoroutine(TemporarilyHideFloor());

            // Inicia a música do boss
            if (bossMusic != null)
                audioManager.TrocarMusica(1, 1f);

            // Remove o trigger para evitar reativação
            GetComponent<BoxCollider2D>().enabled = false;

            // Spawn do boss e barra de vida
            Instantiate(_bossPrefab, _bossPosition.position, Quaternion.identity);
            _bossHealthBar.SetActive(true);
        }
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _animFloorBlocker = floorBlocker.GetComponent<Animator>();
    }

    // Faz o chão desaparecer temporariamente e reaparecer logo depois
    private System.Collections.IEnumerator TemporarilyHideFloor()
    {
        ToggleFloor(false); // Desativa o chão
        yield return new WaitForSeconds(1f); // Espera 1 segundo
        ToggleFloor(true); // Reativa o chão
    }

    // Altera a visibilidade do chão
    private void ToggleFloor(bool isVisible)
    {
        _floorVisible = isVisible;
        floorBlocker.SetActive(isVisible); // Ativa ou desativa o objeto do chão
    }

    public void RemoveFloorBlocker()
    {
        // Desativa o chão permanentemente quando o boss for derrotado
        ToggleFloor(false);

        // Finaliza a música do boss
        audioManager.TrocarMusica(0);

        Destroy(_bossHealthBar);
    }
}
