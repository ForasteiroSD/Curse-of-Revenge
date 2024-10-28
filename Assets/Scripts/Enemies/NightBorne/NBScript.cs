using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class NBScript : MonoBehaviour, InterfaceGetHit
{
    Animator _animator;

    Rigidbody2D _nbRb;

    BoxCollider2D _collider;

    [SerializeField] float _horSpeed = 8f;
    [SerializeField] float _unitsToMove;
    [SerializeField] bool _returnOnlyOnBorder = false;
    float _startPos;
    float _endPos;

    [SerializeField] float _idleTime = 2f;
    bool _idle = false;

    bool _isChasing = false;
    [SerializeField] float _chaseSpeedMultiplier = 1.5f; //starts with 1.5, changes to 1.7 on phase 2

    Transform _player;
    [SerializeField] Transform _maxChasePos;
    [SerializeField] Transform _minChasePos;

    [SerializeField] LayerMask _playerLayer;
    [SerializeField] float _attackDistance = 1.5f;
    [SerializeField] float _attackCooldown = 1.9f; //starts with 1.9, changes to 1.3 on phase 2
    [SerializeField] float _attackDuration = 0.7f; //starts with 0.7, changes to 0.5 on phase 2
    [SerializeField] float _attackDamage = 1f;
    [SerializeField] float _damageReceivedMult = 0.8f;
    int _isAttacking = 1;

    [SerializeField] float _health = 100f;
    [SerializeField] float _hitDelay = 1f;
    bool _hit = false;

    bool _death = false;

    [SerializeField] int _valuePerRevengePoint = 1;
    [SerializeField] int _revengePointsQuantity = 1;

    int _phase = 1;
    [SerializeField] float _dashDistance = 8f;
    [SerializeField] float _dashForce = 50f;
    bool _canDash = true;
    bool _isDashing = false;
    [SerializeField] float _animationSpeed = 1.5f; //starts with 1.5, changes to 2 on phase 2

    bool _changingPhase = false;
    [SerializeField] GameObject _Phase2Effect;
    [SerializeField] GameObject _DashEffect;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _nbRb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAttacking == 0 || _hit || _death || _changingPhase) return;
        
        if (_isChasing)
        {
            Chase();
        }
        else
        {
            Move();
        }
    }
    void Move()
    {
        if (_idle) return;

        if (_horSpeed > 1)
        {
            if (!_returnOnlyOnBorder && _nbRb.position.x > _endPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        else
        {
            if (!_returnOnlyOnBorder && _nbRb.position.x < _startPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        //return from idle animation in case was in it
        _animator.SetBool(Constants.IDLE_ENEMY, false);
        _nbRb.linearVelocityX = _horSpeed;
    }

    public void GetOnBorder()
    {
        _isChasing = false;
        StartCoroutine(Idle());
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
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
    }

    void Chase()
    {
        if (_idle)
        {
            _idle = false;
            _animator.SetBool(Constants.IDLE_ENEMY, false);
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

        if (Mathf.Abs(distance) < _attackDistance && !_changingPhase)
        {
            _nbRb.linearVelocityX = 0;
            StartCoroutine(Attack());
        }
        else 
        {
            if (_isDashing || _changingPhase) return;

            //return from idle animation in case was in it
            _animator.SetBool(Constants.IDLE_ENEMY, false);

            _nbRb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;

            if(_phase > 1 && Mathf.Abs(distance) >= _dashDistance && _canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    public void GetHit(float damage)
    {
        if (_changingPhase) return;

        _health -= damage * _damageReceivedMult;

        if (_health > 0)
        {
            if(_health <= 50 && _phase == 1)
            {
                StartCoroutine(ChangePhase());
            } else
            {
                StartCoroutine(Hit());
            }
        }
        else
        {
            if (!_death) StartCoroutine(Death());
        }

        print(_health);
    }

    IEnumerator ChangePhase()
    {
        _nbRb.linearVelocityX = 0;
        _changingPhase = true;
        GameObject effect = Instantiate(_Phase2Effect, new Vector2(transform.position.x, transform.position.y-1f), Quaternion.identity);
        Destroy(effect, 5f);
        yield return new WaitForSeconds(5);
        _phase++;
        _animationSpeed = 2;
        _attackCooldown = 1.3f;
        _attackDuration = 0.5f;
        _chaseSpeedMultiplier = 1.7f;
        _animator.SetFloat("Speed", _animationSpeed);
        _changingPhase = false;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {

        //stops current moviment
        _nbRb.linearVelocityX = 0;

        //set animation for preparing to dash
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //set that no longer can dash and is dashing
        _canDash = false;
        _isDashing = true;

        yield return new WaitForSeconds(1f);
        
        //if player gets close enought to attack while preparing to dash cancel dash
        if(_isAttacking == 0)
        {
            _isDashing = false;
            _canDash = true;
            yield break;
        }

        //Add dash effect on ground
        GameObject dashEffect = Instantiate(_DashEffect, new Vector2(transform.position.x, transform.position.y - 1.65f), Quaternion.identity);

        //aply dash force
        _nbRb.AddForceX(_dashForce * MathF.Sign(_horSpeed), ForceMode2D.Impulse);

        Destroy(dashEffect, 2f);

        yield return new WaitForSeconds(0.5f);

        //if after dash is not attacking set animation to running
        if(_isAttacking > 0) _animator.SetBool(Constants.IDLE_ENEMY, false);

        _isDashing = false;

        //wait for delay to be able to dash again
        yield return new WaitForSeconds(3f);

        _canDash = true;
    }

    IEnumerator Attack()
    {
        //get into attack mode
        _isAttacking = 0;
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _animator.SetTrigger(Constants.ATTACK_ENEMY);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    public void GiveDamage()
    {
        //print("dano");
        _player.gameObject.GetComponent<Adventurer>().GetHit(_attackDamage);
        //chamar função de dano no player passando _attackDamage
    }

    IEnumerator Idle()
    {
        _nbRb.linearVelocityX = 0;
        _idle = true;
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        yield return new WaitForSeconds(_idleTime);

        if (_isChasing) yield break;

        _idle = false;
        _animator.SetBool(Constants.IDLE_ENEMY, false);
        Flip();
    }

    IEnumerator Hit()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("NB Attack")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _nbRb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    IEnumerator Death()
    {
        _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _nbRb.linearVelocityX = 0;

        yield return new WaitForSeconds(2.25f);

        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<EnemyRevengePoint>().DropRevengePoint(_valuePerRevengePoint, _revengePointsQuantity, transform);
        Destroy(gameObject);


    }
}
