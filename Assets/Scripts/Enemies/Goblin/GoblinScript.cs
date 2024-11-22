using System.Collections;
using UnityEngine;
using Utils;

public class GoblinScript : EnemiesScript
{
    CapsuleCollider2D _collider;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] float _bombDamage = 10f;
    [SerializeField] float _bombCooldown = 10f;
    [SerializeField] float _distanceToUseBomb = 8f;
    [SerializeField] Vector2 _bombForce;
    [SerializeField] Transform _bombPos;
    bool _canUseBomb = true;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        SFXManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected override void Chase()
    {
        if (_idle)
        {
            _idle = false;
            _animator.SetBool(Constants.IDLE_ENEMY, false);
        }

        float distance = _player.transform.position.x - transform.position.x;

        if( distance >= 0 && _horSpeed < 0) Flip();
        else if (distance < 0 && _horSpeed > 0) Flip();
        
        distance = Mathf.Abs(distance);
        if (_canUseBomb && distance <= _distanceToUseBomb && distance >= _distanceToUseBomb-2f)
        {
            StartCoroutine(UseBomb());
            return;
        }

        if (distance < _attackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            //return from idle animation in case was in it
            _animator.SetBool(Constants.IDLE_ENEMY, false);

            _rb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    IEnumerator UseBomb()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        _canUseBomb = false;

        _animator.SetTrigger("Attack2");

        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;
    }

    //called by attack 2 animation
    void ThrowBomb()
    {
        GameObject bomb = Instantiate(_bombPrefab, _bombPos.position, Quaternion.identity);
        bomb.GetComponent<Bomb>().damage = _bombDamage;
        bomb.GetComponent<Rigidbody2D>().AddForce(new Vector2(_bombForce[0]*Mathf.Sign(_horSpeed), _bombForce[1]), ForceMode2D.Impulse);
        StartCoroutine(BombCooldown());
    }

    IEnumerator BombCooldown()
    {
        yield return new WaitForSecondsRealtime(_bombCooldown);
        _canUseBomb = true;
    }

    protected override IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;

        _animator.SetTrigger("Attack1");

        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    protected override IEnumerator Hit()
    {
        string animation = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (animation.CompareTo("GoblinAttack1") == 0 || animation.CompareTo("GoblinAttack2") == 0) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}