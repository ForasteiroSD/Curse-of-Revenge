using UnityEngine;
using Utils;

public class RevengePoint : MonoBehaviour
{
    public int value { get; set; } = 1;

    void Awake()
    {
        float forceX = Random.Range(-2.5f, 2.5f);
        float forceY = Random.Range(1.5f, 3f);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.TAG_PLAYER))
        {
            FindAnyObjectByType<ScoreUI>().UpdateScore(value);
            Destroy(gameObject);
        }
    }
}
