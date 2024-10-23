using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class SkeletonScript : MonoBehaviour
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
    bool _idle = false;

    bool _isChasing = false;
    [SerializeField] float _chaseSpeedMultiplier = 1.1f;

    [SerializeField] Transform _player;
    [SerializeField] Transform _maxChasePos;
    [SerializeField] Transform _minChasePos;

    [SerializeField] float _attackDistance = 1.5f;
    [SerializeField] float _attackCooldown = 1.5f;
    [SerializeField] float _attackDuration = 1.5f;
    [SerializeField] Transform _attackPos;
    int _isAttacking = 1;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _skeletonRb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAttacking == 0) return;

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

    public void PlayerEnterRange()
    {
        if (_player.position.x > _minChasePos.position.x && _player.position.x < _maxChasePos.position.x)
        {
            _isChasing = true;
        }
    }

    public void PlayerInRange()
    {
        if (_player.position.x > _minChasePos.position.x && _player.position.x < _maxChasePos.position.x)
        {
            _isChasing = true;
        }
    }

    void Flip()
    {
        _horSpeed *= -1;
        transform.position = new Vector3(transform.position.x + ((Mathf.Sign(_horSpeed)) * _move), transform.position.y, transform.position.z);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

    void Chase()
    {
        if(_idle)
        {
            _idle = false;
            _animator.SetBool(Constants.IDLE_SKELETON, false);
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

        if (_isAttacking == 1 && Mathf.Abs(distance) < _attackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            _skeletonRb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    IEnumerator Attack()
    {
        //get into attack mode
        _skeletonRb.linearVelocityX = 0;
        _isAttacking = 0;
        _animator.SetTrigger(Constants.ATTACK_SKELETON);

        //wait for attack animation
        yield return new WaitForSeconds(_attackDuration);

        //verify if player receives hit
        Vector3 direction = new Vector3(1, 0, 0);
        if(_horSpeed < 0) direction = new Vector3(-1, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(_attackPos.position, direction, _attackDistance+0.75f);

        if (hit.collider != null && hit.collider == hit.collider.gameObject.CompareTag(Constants.TAG_PLAYER))
        {
            print("dano");
            //chamar função dano player, passando dano
        }

        //change to idle animation
        _animator.SetBool(Constants.IDLE_SKELETON, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

        //return from idle animation
        _animator.SetBool(Constants.IDLE_SKELETON, false);

        
    }

    IEnumerator Idle()
    {
        _skeletonRb.linearVelocityX = 0;
        _idle = true;
        _animator.SetBool(Constants.IDLE_SKELETON, true);

        yield return new WaitForSeconds(2);

        if (_isChasing) yield break;

        _idle = false;
        _animator.SetBool(Constants.IDLE_SKELETON, false);
        Flip();
    }

}