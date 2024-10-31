using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Utils;

public class NBScript : EnemiesScript
{
    [SerializeField] float _SecondPhaseTreshold = 25f;
    int _phase = 1;
    [SerializeField] float _dashDistance = 8f;
    [SerializeField] float _dashForce = 50f;
    bool _canDash = true;
    bool _isDashing = false;
    [SerializeField] float _animationSpeed = 1.5f; //starts with 1.5, changes to 1.7 on phase 2

    bool _changingPhase = false;
    [SerializeField] GameObject _Phase2Effect;
    [SerializeField] GameObject _DashEffect;
    public AudioClip clip;
    public AudioManager audioManager;
    public BossRoomEntry entrances;

    CapsuleCollider2D _collider;
    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_hit || _death || _changingPhase) return;

        if (_isAttacking == 0)
        {
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
            return;
        }

        
        if (_isChasing)
        {
            Chase();
        }
        else
        {
            Move();
        }
    }

    protected override void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
    }

    protected override void Chase()
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
            _rb.linearVelocityX = 0;
            StartCoroutine(Attack());
        }
        else 
        {
            if (_isDashing || _changingPhase) return;

            //return from idle animation in case was in it
            _animator.SetBool(Constants.IDLE_ENEMY, false);

            _rb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;

            if(_phase > 1 && Mathf.Abs(distance) >= _dashDistance && _canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    public override void GetHit(float damage)
    {
        if (_changingPhase || _health < 0) return;

        _health -= damage * _damageReceivedMult;

        Vector3 position = GetTextPosition();
        GameObject text = Instantiate(_textDamage, position, Quaternion.identity);

        text.GetComponent<TextMeshPro>().text = damage.ToString();

        Destroy(text, 2f);

        if (_health > 0)
        {
            if(_health <= _SecondPhaseTreshold && _phase == 1)
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
    }

    IEnumerator ChangePhase()
    {
        _rb.linearVelocityX = 0;
        _changingPhase = true;
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        GameObject effect = Instantiate(_Phase2Effect, new Vector2(transform.position.x, transform.position.y-1f), Quaternion.identity);
        Destroy(effect, 5f);
        yield return new WaitForSeconds(5);
        _phase++;
        _animationSpeed = 1.7f;
        _attackCooldown = 1.8f;
        _chaseSpeedMultiplier = 1.7f;
        _attackDistance = 1.8f;
        _animator.SetFloat("Speed", _animationSpeed);
        Chase();
        StartCoroutine(Dash());
        yield return new WaitForSeconds(1);
        _changingPhase = false;
    }

    IEnumerator Dash()
    {
        //set that no longer can dash and is dashing
        _canDash = false;
        _isDashing = true;

        //stops current moviment
        _rb.linearVelocityX = 0;

        //set animation for preparing to dash
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        yield return new WaitForSeconds(1f);

        //if player gets close enought to attack while preparing to dash cancel dash
        if (_isAttacking == 0)
        {
            _isDashing = false;
            _canDash = true;
            yield break;
        }

        //Add dash effect on ground
        GameObject dashEffect = Instantiate(_DashEffect, new Vector2(transform.position.x, transform.position.y - 1.65f), Quaternion.identity);

        //aply dash force
        _rb.AddForceX(_dashForce * MathF.Sign(_horSpeed), ForceMode2D.Impulse);

        Destroy(dashEffect, 2f);

        yield return new WaitForSeconds(0.5f);

        //if after dash is not attacking set animation to running
        if (_isAttacking > 0) _animator.SetBool(Constants.IDLE_ENEMY, false);

        _isDashing = false;

        //wait for delay to be able to dash again
        yield return new WaitForSeconds(3f);

        _canDash = true;
    }

    protected override IEnumerator Death()
    {
        _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _rb.linearVelocityX = 0;

        yield return new WaitForSecondsRealtime(2.15f);

        DropRevengePoint();
        audioManager.TrocarMusica(clip);
        entrances.RemoveWallBlocker();
        Destroy(gameObject);
    }

    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}
