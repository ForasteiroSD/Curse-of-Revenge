using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using Utils;

public class FlyingEyeScript : MonoBehaviour, InterfaceGetHit
{
    Animator _animator;

    Rigidbody2D _eyeRb;

    CircleCollider2D _collider;

    [SerializeField] float _horSpeed = 2f;
    [SerializeField] float _verSpeed = 2f;
    [SerializeField] float _unitsToMove;
    float _startPos;
    float _endPos;

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _flyHeightPos;
    bool _goingUp = false;
    bool _goingDown = false;

    [SerializeField] float _idleTime = 2f;
    bool _idle = false;

    bool _isChasing = false;
    [SerializeField] float _chaseSpeedMultiplier = 1.1f;

    Transform _player;
    [SerializeField] Transform _maxChasePos;
    [SerializeField] Transform _minChasePos;

    [SerializeField] LayerMask _playerLayer;
    [SerializeField] float _attackDistance = 1.5f;
    [SerializeField] float _attackCooldown = 1.5f;
    [SerializeField] Transform _attackPos;
    [SerializeField] float _attackDamage = 1f;
    [SerializeField] float _damageReceivedMult = 0.8f;
    int _isAttacking = 1;

    [SerializeField] float _health = 20f;
    [SerializeField] float _hitDelay = 1f;
    bool _hit = false;

    bool _death = false;

    [SerializeField] int _valuePerRevengePoint = 1;
    [SerializeField] int _revengePointsQuantity = 1;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _eyeRb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (_death) return;

        if (_goingUp)
        {
            _eyeRb.linearVelocityY = _verSpeed;

            Vector3 direction = new Vector3(1, 0, 0);
            if (_horSpeed < 0) direction = new Vector3(-1, 0, 0);

            RaycastHit2D hit = Physics2D.Raycast(_flyHeightPos.position, direction, 1.5f, _groundLayer);

            if (hit.collider == null)
            {
                _eyeRb.linearVelocityY = 0.5f;
                _goingUp = false;
            }

        } else if (_goingDown)
        {
            _eyeRb.linearVelocityY = -_verSpeed;

            Vector3 direction = new Vector3(0, -1, 0);

            RaycastHit2D hit = Physics2D.Raycast(_flyHeightPos.position, direction, 0.1f, _groundLayer);

            if (hit.collider != null)
            {
                _eyeRb.linearVelocityY = 0f;
                _goingDown = false;
            }
        }
        else
        {
            _eyeRb.linearVelocityY = 0;

            if (_isAttacking == 0 || _hit) return;

            if (_isChasing)
            {
                Chase();
            }
            else
            {
                Move();
            }
        }
    }
    void Move()
    {
        if(_idle) return;

        if (_horSpeed > 1)
        {
            if (_eyeRb.position.x > _endPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        else
        {
            if (_eyeRb.position.x < _startPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        _eyeRb.linearVelocityX = _horSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_goingDown) return;
        _eyeRb.linearVelocityX = 0;
        _eyeRb.linearVelocityY = 0;
        Vector3 direction = new Vector3(1, 0, 0);
        if (_horSpeed < 0) direction = new Vector3(-1, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1f), direction, 1.5f, _groundLayer);

        //if there's a wall in front and it's height is greater then height the eye can go up
        if (hit.collider != null)
        {
            _isChasing = false;
            StartCoroutine(Idle());
        }
        else
        {
            _goingUp = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _eyeRb.linearVelocityY = 0;
    }

    public void GoDown()
    {
        if (!_goingUp)
        {
            _eyeRb.linearVelocityX = 0;

            Vector3 direction = new Vector3(0, -1, 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3f, _groundLayer);

            //if there's ground bellow the eye and it's not too deep
            if (hit.collider != null)
            {
                _goingDown = true;
            }
            else
            {
                StartCoroutine(Idle());
            }
        }
    }

    public void PlayerLeaveRange()
    {
        if (_player.position.x < _minChasePos.position.x || _player.position.x > _maxChasePos.position.x)
        {
            _isChasing = false;
        }
    }

    public void PlayerInRange()
    {
        if (_player.position.x > _minChasePos.position.x && _player.position.x < _maxChasePos.position.x)
        {
            _isChasing = true;
        }
        else
        {
            _isChasing = false;
        }
    }

    void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

    void Chase()
    {
        if (_idle)
        {
            _idle = false;
        }

        float distance = _player.transform.position.x - transform.position.x;

        if (distance > 0)
        {
            if (_horSpeed < 0)
            {
                Flip();
            }
        }
        else
        {
            if (_horSpeed > 0)
            {
                Flip();
            }
        }

        if (Mathf.Abs(distance) < _attackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            _eyeRb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    public void GetHit(float damage)
    {
        _health -= damage * _damageReceivedMult;

        if (_health > 0)
        {
            StartCoroutine(Hit());
        }
        else
        {
            if(!_death) StartCoroutine(Death());
        }

        print(_health);
    }
    IEnumerator Attack()
    {
        //get into attack mode
        _eyeRb.linearVelocityX = 0;
        _isAttacking = 0;
        _animator.SetTrigger(Constants.ATTACK_ENEMY);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    public void GiveDamage()
    {
        print("dano");
        //chamar função de dano no player passando _attackDamage
    }

    IEnumerator Idle()
    {
        _eyeRb.linearVelocityX = 0;
        _idle = true;

        yield return new WaitForSeconds(_idleTime);

        if (_isChasing) yield break;

        _idle = false;
        Flip();
    }

    IEnumerator Hit()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Flying Eye Attack")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _eyeRb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    IEnumerator Death()
    {
        _death = true;
        _collider.offset = new Vector2(_collider.offset.x, -0.15f);
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _eyeRb.linearVelocityX = 0;
        _eyeRb.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(2);

        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<EnemyRevengePoint>().DropRevengePoint(_valuePerRevengePoint, _revengePointsQuantity, transform);

        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}
