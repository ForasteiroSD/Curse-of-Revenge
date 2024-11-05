using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

public class EnemiesScript : MonoBehaviour
{
    protected Animator _animator;

    protected Rigidbody2D _rb;

    [SerializeField] protected float _horSpeed = 2f;
    [SerializeField] protected float _unitsToMove;
    [SerializeField] protected bool _returnOnlyOnBorder = false;
    protected float _startPos;
    protected float _endPos;

    [SerializeField] protected float _idleTime = 2f;
    protected bool _idle = false;

    protected bool _isChasing = false;
    [SerializeField] protected float _chaseSpeedMultiplier = 1.1f;

    protected Transform _player;
    [SerializeField] protected Transform _maxChasePos;
    [SerializeField] protected Transform _minChasePos;

    [SerializeField] protected LayerMask _playerLayer;
    [SerializeField] protected float _attackDistance = 1.5f;
    [SerializeField] protected float _attackCooldown = 1.5f;
    [SerializeField] protected float _attackDamage = 1f;
    [SerializeField] protected float _damageReceivedMult = 0.8f;
    protected int _isAttacking = 1;

    [SerializeField] protected float _health = 20f;
    [SerializeField] protected float _hitDelay = 1f;
    protected bool _hit = false;

    protected bool _death = false;

    [SerializeField] protected int _valuePerRevengePoint = 1;
    [SerializeField] protected int _revengePointsQuantity = 1;

    [SerializeField] private GameObject _revengePoint;
    [SerializeField] protected GameObject _textDamage;
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    protected virtual void Update()
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
    protected virtual void Move()
    {
        if (_idle) return;

        if (_horSpeed > 1)
        {
            if (!_returnOnlyOnBorder && _rb.position.x > _endPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        else
        {
            if (!_returnOnlyOnBorder && _rb.position.x < _startPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        //return from idle animation in case was in it
        _animator.SetBool(Constants.IDLE_ENEMY, false);
        _rb.linearVelocityX = _horSpeed;
    }

    public void GetOnBorder()
    {
        _rb.linearVelocityX = 0;
        _isChasing = false;
        StartCoroutine(Idle());
    }

    public void PlayerLeaveRange()
    {
        _isChasing = false;
    }

    public virtual void PlayerInRange()
    {
        if (_player.position.x > _minChasePos.position.x && _player.position.x < _maxChasePos.position.x)
        {
            _isChasing = true;
        }
        else
        {
            _rb.linearVelocityX = 0;
            _isChasing = false;
        }
    }

    protected virtual void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

    protected virtual void Chase()
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

        if (Mathf.Abs(distance) < _attackDistance)
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

    public virtual void GetHit(float damage)
    {
        if(_death) return;

        _health -= damage * _damageReceivedMult;

        Vector3 position = GetTextPosition();
        GameObject text = Instantiate(_textDamage, position, Quaternion.identity);

        text.GetComponent<TextMeshPro>().text = damage.ToString();

        if (_health > 0)
        {
            StartCoroutine(Hit());
        }
        else
        {
            if (!_death) StartCoroutine(Death());
        }
    }

    protected virtual Vector3 GetTextPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
    }

    protected virtual IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        _animator.SetTrigger(Constants.ATTACK_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    public void GiveDamage()
    {
        _player.gameObject.GetComponent<Adventurer>().GetHit(_attackDamage);
    }

    protected virtual IEnumerator Idle()
    {
        _rb.linearVelocityX = 0;
        _idle = true;
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        yield return new WaitForSeconds(_idleTime);

        if (_isChasing) yield break;

        _idle = false;
        _animator.SetBool(Constants.IDLE_ENEMY, false);
        Flip();
    }

    protected virtual IEnumerator Hit()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy Attack")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    protected virtual IEnumerator Death()
    {
        _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(1);

        DropRevengePoint();

        yield return new WaitForSeconds(3);


        Destroy(gameObject);
    }

    protected void DropRevengePoint()
    {
        for (int i = 0; i < _revengePointsQuantity; i++)
        {
            float posX = Random.Range(-1.5f, 1.5f);
            GameObject revengePoint = Instantiate(_revengePoint, new Vector3(transform.position.x + posX, transform.position.y, transform.position.z), Quaternion.identity);
            revengePoint.GetComponent<RevengePoint>().value = _valuePerRevengePoint;
        }
    }
}
