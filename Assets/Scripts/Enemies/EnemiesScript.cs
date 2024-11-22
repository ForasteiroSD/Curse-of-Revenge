using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

public class EnemiesScript : MonoBehaviour
{
    //Referencies
    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected Transform _player;
    [SerializeField] public Transform _maxChasePos;
    [SerializeField] public Transform _minChasePos;
    [SerializeField] private GameObject _revengePoint;
    [SerializeField] protected GameObject _textDamage;


    //Status
    [SerializeField] public float _health = 20f;
    [SerializeField] public float _horSpeed = 2f;
    [SerializeField] protected float _idleTime = 2f;
    [SerializeField] protected float _chaseSpeedMultiplier = 1.1f;
    [SerializeField] protected float _attackDistance = 1.5f;
    [SerializeField] protected float _attackCooldown = 1.5f;
    [SerializeField] protected float _attackDamage = 1f;
    [SerializeField] protected float _damageReceivedMult = 0.8f;
    [SerializeField] protected float _hitDelay = 1f;
    [SerializeField] protected int _valuePerRevengePoint = 1;
    [SerializeField] protected int _revengePointsQuantity = 1;
    [SerializeField] protected float _move = 0.2f;

    //States
    protected bool _idle = false;
    protected bool _isChasing = false;
    protected int _isAttacking = 1;
    protected bool _hit = false;
    protected bool _death = false;
    protected bool _isOnBorder = false;
    protected bool _attackedOnBorder = false;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected virtual void Update()
    {
        if (_isAttacking == 0 || _hit || _death) return;

        if (_attackedOnBorder && _isAttacking == 1)
        {
            StartCoroutine(Idle());
            _attackedOnBorder = false;
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
    protected virtual void Move()
    {
        if (_idle) return;

        if (transform.position.x <= _minChasePos.position.x || transform.position.x >= _maxChasePos.position.x) {
            StartCoroutine(Idle());
            return;
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

        if (_player.position.x >= _minChasePos.position.x && _player.position.x <= _maxChasePos.position.x)
        {
            _isChasing = true;
            return;
        }

        if(_isOnBorder && _isAttacking == 1 && !_hit && !_death)
        {
            float distanceMax = _maxChasePos.position.x - transform.position.x;
            float distanceMin = transform.position.x - _minChasePos.position.x;
            int border = distanceMax > distanceMin ? 0 : 1;
            if (border == 0 && _player.position.x <= _minChasePos.position.x && _player.position.x >= _minChasePos.position.x - 1)
            {
                float distance = _player.transform.position.x - transform.position.x;

                if(distance >= 0 && _horSpeed < 0) Flip();
                else if(distance < 0 && _horSpeed > 0) Flip();

                StartCoroutine(Attack());
                _attackedOnBorder = true;
            }
            else if (border == 1 && _player.position.x >= _maxChasePos.position.x && _player.position.x <= _maxChasePos.position.x + 1)
            {
                float distance = _player.transform.position.x - transform.position.x;

                if(distance >= 0 && _horSpeed < 0) Flip();
                else if(distance < 0 && _horSpeed > 0) Flip();

                StartCoroutine(Attack());
                _attackedOnBorder = true;
            }
        }

        _isChasing = false;
    }

    protected virtual void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.position = new Vector3(transform.position.x + Mathf.Sign(_horSpeed) * _move, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
    }

    protected virtual void Chase()
    {
        if (_idle)
        {
            _idle = false;
            _animator.SetBool(Constants.IDLE_ENEMY, false);
        }

        float distance = _player.transform.position.x - transform.position.x;

        if(distance >= 0 && _horSpeed < 0) Flip();
        else if(distance < 0 && _horSpeed > 0) Flip();

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

        damage *= _damageReceivedMult;
        damage = Mathf.Ceil(damage);
        _health -= damage;

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
        _isOnBorder = true;
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        yield return new WaitForSeconds(_idleTime);

        if (_isChasing || _isAttacking == 0) yield break;

        _isOnBorder = false;
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

        yield return new WaitForSeconds(Constants.REVENGE_POINT_DROP_TIME);

        DropRevengePoint();

        Destroy(transform.parent.gameObject, 4);
    }

    protected void DropRevengePoint()
    {
        for (int i = 0; i < _revengePointsQuantity; i++)
        {
            GameObject revengePoint = Instantiate(_revengePoint, transform.position, Quaternion.identity);
            revengePoint.GetComponent<RevengePoint>().value = _valuePerRevengePoint;
        }
    }
}
