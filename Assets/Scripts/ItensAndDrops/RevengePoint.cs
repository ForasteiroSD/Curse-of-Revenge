using UnityEngine;
using Utils;

public class RevengePoint : MonoBehaviour
{
    public int value { get; set; } = 1;
    public AudioManager SFXManager;

    void Awake()
    {
        float forceX = Random.Range(-2.5f, 2.5f);
        float forceY = Random.Range(1.5f, 3f);
        SFXManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.TAG_PLAYER))
        {
            SFXManager.TocarSFX(13);
            FindAnyObjectByType<ScoreUI>().UpdateScore(value);
            Destroy(gameObject);
        }
    }
}
