using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class SkeletonScript : MonoBehaviour, InterfaceGetHit
{
    Animator _animator;

    Rigidbody2D _skeletonRb;

    BoxCollider2D _collider;

    [SerializeField] float _horSpeed = 2f;
    [SerializeField] float _unitsToMove;
    [SerializeField] bool _returnOnlyOnBorder = false;
    float _startPos;
    float _endPos;

    [SerializeField] float _move = 0.5f;
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
        _skeletonRb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAttacking == 0 || _hit || _death) return;

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
            if (!_returnOnlyOnBorder && _skeletonRb.position.x > _endPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        else
        {
            if (!_returnOnlyOnBorder && _skeletonRb.position.x < _startPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        //return from idle animation in case was in it
        _animator.SetBool(Constants.IDLE_ENEMY, false);
        _skeletonRb.linearVelocityX = _horSpeed;
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
        transform.position = new Vector3(transform.position.x + ((Mathf.Sign(_horSpeed)) * _move), transform.position.y, transform.position.z);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

    void Chase()
    {
        if(_idle)
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

        if (Mathf.Abs(distance) < _attackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            //return from idle animation in case was in it
            _animator.SetBool(Constants.IDLE_ENEMY, false);

            _skeletonRb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    public void GetHit(float damage)
    {
        _health -= damage*_damageReceivedMult;

        if (_health > 0)
        {
            StartCoroutine(Hit());
        }
        else
        {
            if (!_death) StartCoroutine(Death());
        }

        print(_health);
    }

    IEnumerator Attack()
    {
        //get into attack mode
        _skeletonRb.linearVelocityX = 0;
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
        _skeletonRb.linearVelocityX = 0;
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
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Skeleton Attack")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _skeletonRb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    IEnumerator Death()
    {
        _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _skeletonRb.linearVelocityX = 0;

        yield return new WaitForSeconds(2);

        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<EnemyRevengePoint>().DropRevengePoint(_valuePerRevengePoint, _revengePointsQuantity, transform);

        yield return new WaitForSeconds(3);


        Destroy(gameObject);
    }

}