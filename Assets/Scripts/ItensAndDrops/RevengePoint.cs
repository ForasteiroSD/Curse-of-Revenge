using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RevengePoint : MonoBehaviour
{

    public int value { get; set; } = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        FindAnyObjectByType<ScoreUI>().UpdateScore(value);
        Destroy(gameObject);
    }
}
