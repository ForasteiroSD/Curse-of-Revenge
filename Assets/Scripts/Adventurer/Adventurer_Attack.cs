using UnityEngine;
using Utils;

public class Adventurer_Attack : MonoBehaviour
{
    //Scripts
    private Adventurer _adventurer;

    //Components
    private Rigidbody2D _rb;

    //Attack
    private int _attackCounter = 0;
    private bool _isAttacking = false;
    private Animator _animator;
    private float _lastAttackTime = -10;
    [SerializeField] private float _preAttackTimeLimit = .2f;
    [SerializeField] private float _attackCooldown = .1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _adventurer = GetComponent<Adventurer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(_isAttacking && !AnimatorIsPlaying("Adventurer_Attack_" + _attackCounter))
        {
            if (_attackCounter == 3) _attackCounter = 0;
            _isAttacking = false;

            if (Time.time <= _lastAttackTime + _preAttackTimeLimit) OnAttack();
            else _adventurer._canMove = true;
        }
    }

    void OnAttack()
    {
        _lastAttackTime = Time.time;
        if (!_isAttacking && !_adventurer._isSliding && !_adventurer._canWallJump && _adventurer._canJump)
        {
            _isAttacking = true;
            _adventurer._canMove = false;
            _rb.linearVelocityX = 0;
            _animator.SetInteger(Constants.ANIM_ATTACK_COUNTER, ++_attackCounter);
            _animator.SetTrigger(Constants.ANIM_ATTACK);
        }
    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime - _attackCooldown;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}