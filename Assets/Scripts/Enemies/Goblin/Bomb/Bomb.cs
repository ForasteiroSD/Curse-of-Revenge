using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Transform _player;
    Rigidbody2D _rb;
    public float damage;
    protected AudioManager SFXManager;

    void Awake()
    {
        SFXManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, 7);
    }

    void PlayAudio()
    {
        SFXManager.TocarSFX(18);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _rb.linearVelocityX /= 2;
        _rb.linearVelocityY /= 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.activeInHierarchy) {
                _player.gameObject.GetComponent<Adventurer>().GetHit(damage);
            }
        }
    }
}