using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Utils;

public class FireKnightScript : BossScript
{
    //Referencies
    Animator _animator;
    Rigidbody2D _rb;
    CapsuleCollider2D _collider;
    [SerializeField] Transform _knightPos;
    [SerializeField] private GameObject _revengePoint;
    [SerializeField] GameObject _textDamage;
    [SerializeField] LayerMask _groundLayer;
    Transform _player;
    
    //Status
    [SerializeField] int _valuePerRevengePoint = 1;
    [SerializeField] int _revengePointsQuantity = 1;

    [SerializeField] float _horSpeed = 2f;
    [SerializeField] float _chaseSpeedMultiplier = 1.1f;

    [SerializeField] float _attackDistance = 1.5f;
    [SerializeField] float _attackCooldown = 1.5f;
    [SerializeField] float _attackSPCooldown = 5f;

    [SerializeField] float _distanceToUseAirAttack = 2.5f;
    [SerializeField] float _airAttackJumpForce = 12f;

    [SerializeField] float _rollingForce = 12f;

    [SerializeField] float _damageReceivedMult = 0.8f;
    [SerializeField] float _health = 20f;
    [SerializeField] float _hitDelay = 1f;
    [SerializeField] float[] _attackDamages;
    [SerializeField] float _dodgeChange = 0.7f;
    [SerializeField] int _maxDeffendCount = 2;

    //States
    bool _idle = false;
    int _isAttacking = 1;
    bool _giveHit = false;
    bool _death = false;
    bool _rolling = false;
    bool _defend = false;
    bool _canUseSPAttack = false;
    bool _isUsingSPAttack = false;
    int _currentAttack = 0;
    int _deffendCount = 0;
    bool _isUsingFirstAttack = false;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        StartCoroutine(SPAttackCooldown());
    }

    void Update()
    {
        if (_isUsingSPAttack || _isUsingFirstAttack || _defend)
        {
            Vector2 distance = _player.transform.position - _knightPos.position;

            if(distance.x >= 0 && _horSpeed < 0) Flip();
            else if(distance.x < 0 && _horSpeed > 0) Flip();
            return;
        }

        if (_idle || _isAttacking == 0) return;

        Chase();
    }

    void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

    void Chase()
    {
        Vector2 distance = _player.transform.position - _knightPos.position;

        if (distance.x >= 0 && _horSpeed < 0) Flip();
        else if (distance.x < 0 && _horSpeed > 0) Flip();

        if (Mathf.Abs(distance.x) < _attackDistance) Attack(distance.y);
        else
        {
            //return from idle animation in case was in it
            _animator.SetBool("Idle", false);

            _rb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    //called by adventurer
    public override void GetHit(float damage)
    {
        if(_death) return;

        float defending = 1;
        if (_defend) defending = 0.5f;

        damage *= defending * _damageReceivedMult;

        damage = Mathf.Ceil(damage);
        
        _health -= damage;

        Vector3 position = GetTextPosition();
        GameObject text = Instantiate(_textDamage, position, Quaternion.identity);

        text.GetComponent<TextMeshPro>().text = damage.ToString();

        if (_health > 0) StartCoroutine(Hit());
        else if (!_death) StartCoroutine(Death());
    }

    Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }

    void Attack(float heightDistance)
    {
        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        _defend = false;
        _rolling = false;
        _isUsingSPAttack = false;
        _idle = true;
        _deffendCount = 0;

        if (heightDistance >= _distanceToUseAirAttack) {
            _currentAttack = 5;
            _animator.SetBool("AirAttack", true);
        } else {
            if (_canUseSPAttack) {
                _currentAttack = 4;
                _isUsingSPAttack = true;
                _animator.SetTrigger("SPAttack");
                _canUseSPAttack = false;
            } else {
                _giveHit = false;
                _isUsingFirstAttack = true;
                _animator.SetTrigger("Attack");
                _currentAttack = 1;
            }
        }

    }

    void StopFirstAttack()
    {
        _isUsingFirstAttack = false;
    }

    //called by event on air attack animation
    void JumpAirAttack()
    {
        _rb.AddForceY(_airAttackJumpForce, ForceMode2D.Impulse);
    }

    //called by event on air attack animation
    void StopAirFalling()
    {
        if (_collider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            _animator.SetBool("AirAttack", false);
            _isAttacking = 1;

            StartCoroutine(IdleTimeout(0.5f));
        };
    }

    //called by event on special attack animation
    void StopSpecialAttack()
    {
        _isUsingSPAttack = false;
        StartCoroutine(SPAttackCooldown());
    }

    IEnumerator SPAttackCooldown()
    {
        yield return new WaitForSecondsRealtime(_attackSPCooldown);
        _canUseSPAttack = true;
    }

    //called by events on attack animations
    void GoToNextAttack()
    {
        if (_giveHit) {
            _currentAttack++;
            Vector2 distance = _player.transform.position - _knightPos.position;
            if(distance.x >= 0 && _horSpeed < 0) Flip();
            else if (distance.x < 0 && _horSpeed > 0) Flip();
            _giveHit = false;
            _animator.SetTrigger("Attack");
        } else {
            _currentAttack = 0;
            StopAttack();
        }
    }

    //also called by attack 3 animation
    void StopAttack()
    {
        _isAttacking = 1;

        StartCoroutine(IdleTimeout(_attackCooldown));
    }

    //called by input system
    void OnAttack()
    {

        if (_isAttacking == 0 || _rolling || _defend || !_idle) return;

        Vector2 distance = _player.transform.position - _knightPos.position;
        if (Mathf.Abs(distance.x) < _attackDistance) {
            if (Random.Range(0f, 1f) <= _dodgeChange) {
                Vector3 direction = new Vector3(1, 0, 0);
                if (_horSpeed > 0) direction = new Vector3(-1, 0, 0);

                RaycastHit2D hit = Physics2D.Raycast(_knightPos.position, direction, 2.5f, _groundLayer);

                if(_isAttacking == 0) return;

                _idle = true;
                if (hit.collider == null)
                {
                    _rolling = true;
                    _animator.SetTrigger("Roll");
                } else
                {
                    if(_deffendCount == _maxDeffendCount) {
                        _rolling = true;
                        _animator.SetTrigger("Roll");
                        return;
                    }
                    _deffendCount++;
                    _defend = true;
                    _animator.SetTrigger("Defend");
                }
                _animator.SetBool("Idle", true);
            }
        }
    }

    //called by defend animation
    void StopDefend()
    {
        _defend = false;
        StartCoroutine(IdleTimeout(_hitDelay));
    }

    //called by rolling animation
    void StartRoll()
    {
        if(_deffendCount != _maxDeffendCount) {
            Vector2 distance = _player.transform.position - _knightPos.position;
            if(distance.x >= 0 && _horSpeed > 0) Flip();
            else if(distance.x < 0 && _horSpeed < 0) Flip();
        }
        _deffendCount = 0;
        _rb.AddForceX(_rollingForce * Mathf.Sign(_horSpeed), ForceMode2D.Impulse);
    }

    //called by rolling animation
    void StopRolling()
    {
        _rolling = false;
        Flip();
        StartCoroutine(IdleTimeout(_hitDelay));
    }

    IEnumerator IdleTimeout(float delay)
    {

        _animator.SetBool("Idle", true);

        yield return new WaitForSecondsRealtime(delay);

        if (_rolling || _defend || _isAttacking == 0) yield break;
        
        _idle = false;

    }

    public void GiveDamage()
    {
        _player.gameObject.GetComponent<Adventurer>().GetHit(_attackDamages[_currentAttack-1]);
        _giveHit = true;
    }

    IEnumerator Hit()
    {
        if (_isAttacking == 0 || _rolling || _defend) yield break;

        _idle = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _rb.linearVelocityX = 0;

        StartCoroutine(IdleTimeout(_hitDelay));
    }

    IEnumerator Death()
    {
        _idle = _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(Constants.REVENGE_POINT_DROP_TIME);

        DropRevengePoint();

        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }

    void DropRevengePoint()
    {
        for (int i = 0; i < _revengePointsQuantity; i++)
        {
            GameObject revengePoint = Instantiate(_revengePoint, transform.position, Quaternion.identity);
            revengePoint.GetComponent<RevengePoint>().value = _valuePerRevengePoint;
        }
    }
}
