using System;
using UnityEngine;
using System.Collections; // Necessário para IEnumerator
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class AutoMoveTrigger : MonoBehaviour
{
    public float autoMoveSpeed = 5f; // Velocidade de movimento automático do player
    public Adventurer adventurer;
    private Rigidbody2D rb;
    public CinemachineCamera _camera;
    [SerializeField] private int nextSceneNumber;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Define o estado do player como "morto" (ou sem controle) usando o método público
            adventurer = other.GetComponent<Adventurer>();
            adventurer.SetIsDead(true);
            _camera.Target.TrackingTarget = null;
            // Inicia a movimentação automática
            StartCoroutine(AutoMovePlayer(other.gameObject));
            print("entrou");
            StartCoroutine(TrocaLevel());
        }
    }

    private void Start()
    {
        adventurer = GameObject.FindGameObjectWithTag("Player").GetComponent<Adventurer>();
        rb = adventurer.GetComponent<Rigidbody2D>();
        
        GameObject cameraObject = GameObject.FindGameObjectWithTag("Cinemachine");
        
        _camera = cameraObject.GetComponent<CinemachineCamera>();

    }

    private IEnumerator TrocaLevel()
    {
        GameManager _gameManager = FindFirstObjectByType<GameManager>();
        yield return new WaitForSecondsRealtime(1.5f);
        _gameManager._level = nextSceneNumber;
        _gameManager.SaveGame();
        StartCoroutine(_gameManager.LoadScene(nextSceneNumber));
    }

    private IEnumerator AutoMovePlayer(GameObject player)
    {
        if (rb != null)
        {
            while (true) // Continua enquanto o player não recuperar o controle
            {
                if (player.transform.localScale.x < 0)
                {
                    player.transform.localScale = new Vector2(player.transform.localScale.x * (-1), player.transform.localScale.y);
                }
                rb.linearVelocity = new Vector2(autoMoveSpeed, rb.linearVelocity.y); // Substituído por linearVelocity
                yield return null; // Aguarda o próximo frame
            }
        }
    }
}