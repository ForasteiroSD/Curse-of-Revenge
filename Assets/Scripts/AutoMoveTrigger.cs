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
    public CinemachineCamera camera;
    [SerializeField] private string nextSceneName;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Define o estado do player como "morto" (ou sem controle) usando o método público
            adventurer = other.GetComponent<Adventurer>();
            adventurer.SetIsDead(true);
            camera.Target.TrackingTarget = null;
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
        
        camera = cameraObject.GetComponent<CinemachineCamera>();

    }

    private IEnumerator TrocaLevel()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(nextSceneName);
        
    }

    private IEnumerator AutoMovePlayer(GameObject player)
    {
        if (rb != null)
        {
            while (true) // Continua enquanto o player não recuperar o controle
            {
                rb.linearVelocity = new Vector2(autoMoveSpeed, rb.linearVelocity.y); // Substituído por linearVelocity
                yield return null; // Aguarda o próximo frame
            }
        }
    }
}