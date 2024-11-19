using UnityEngine;
using Utils;

public class Adventurer_SpecialAttack : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Transform _player;
    private Animator _animator;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _damage = 6;

    private void Awake() {
        //Getting components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.Find(Constants.HIERARCHY_PLAYER).transform;

        //Setting variables considering player looking direction
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * Mathf.Sign(_player.localScale.x), scale.y, scale.z);
        _speed *= Mathf.Sign(_player.localScale.x);
    }

    private void FixedUpdate() {
        _rb.linearVelocityX = _speed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag(Constants.TAG_ENEMY) || other.CompareTag(Constants.TAG_GROUND))  {
            _speed = 0;
            _animator.SetTrigger("Hit");

            if (other.CompareTag(Constants.TAG_ENEMY)) {
                EnemiesScript script = other.gameObject.GetComponent<EnemiesScript>();
                if(script) script.GetHit(_damage);
                else other.gameObject.GetComponent<BossScript>().GetHit(_damage);
                
            }
        }
        
    }

    private void SelfDestroy() {
        Destroy(this.gameObject);
    }
}
