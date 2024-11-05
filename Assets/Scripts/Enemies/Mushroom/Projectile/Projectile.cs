using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Animator _animator;
    Transform _player;
    [SerializeField] float _destroyTime;
    [SerializeField] float _moveSpeed;
    public float damage;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    void Start()
    {
        StartCoroutine(DestroyProjectile(_destroyTime));
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.position, Time.deltaTime * _moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.activeInHierarchy) {
                _moveSpeed = 0;
                _player.gameObject.GetComponent<Adventurer>().GetHit(damage);

                StartCoroutine(DestroyProjectile(0));
            }
        }
    }

    public IEnumerator DestroyProjectile(float time) {

        yield return new WaitForSeconds(time);

        _moveSpeed = 0;

        _animator.SetTrigger("Break");

        Destroy(gameObject, _animator.GetCurrentAnimatorStateInfo(0).length);

    }


}