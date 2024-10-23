using Unity.VisualScripting;
using UnityEngine;

public class RevengePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        print(collider.gameObject.name);
        if (collider.CompareTag("Player"))
        {
            FindObjectOfType<ScoreUI>().UpdateScore();
            Destroy(gameObject);
        }
    }
}
