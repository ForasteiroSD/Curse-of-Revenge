using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints; // Pontos de movimento
    [SerializeField] private float speed = 2f; // Velocidade de movimento
    [SerializeField] private float checkDistance = 0.1f; // Distância mínima para trocar de waypoint
    [SerializeField] private float movementOffset = 0f; // Offset inicial (em segundos)

    private Transform targetWaypoint; // Próximo ponto
    private int currentWaypointIndex = 0; // Índice do ponto atual
    private float movementTimer; // Temporizador para lidar com o offset inicial

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogError("Nenhum waypoint foi configurado!");
            enabled = false;
            return;
        }

        // Inicializa o primeiro waypoint como o alvo
        targetWaypoint = waypoints[0];
        movementTimer = movementOffset; // Aplica o offset inicial
    }

    void Update()
    {
        // Pausa o movimento até o offset inicial acabar
        if (movementTimer > 0)
        {
            movementTimer -= Time.deltaTime;
            return;
        }

        // Move a plataforma em direção ao waypoint atual
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.deltaTime
        );

        // Verifica se a plataforma chegou perto o suficiente do waypoint
        if (Vector2.Distance(transform.position, targetWaypoint.position) <= checkDistance)
        {
            targetWaypoint = GetNextWaypoint();
        }
    }

    private Transform GetNextWaypoint()
    {
        // Incrementa o índice do waypoint
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0; // Volta ao início para criar um loop
        }

        return waypoints[currentWaypointIndex];
    }
}