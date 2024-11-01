using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class RevengePoint : MonoBehaviour
{

    public int value { get; set; } = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.TAG_PLAYER))
        {
            FindAnyObjectByType<ScoreUI>().UpdateScore(value);
            Destroy(gameObject);
        }
    }
}
