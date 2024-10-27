using System.Collections;
using System.Linq;
using UnityEngine;
using Utils;

public class Adventurer_Attack : MonoBehaviour
{
    //Scripts
    private Adventurer _adventurer;

    //Components
    private Rigidbody2D _rb;
    private Animator _animator;

    //Attack
    private int _attackCounter = 0;
    private bool _attackEnded = false;
    private float _lastAttackAttmeptTime = -10;
    private float _lastAttackTime = 0;
    private float _originalMoveSpeed;
    private GameObject[] _targets;
    [SerializeField] private float _preAttackTimeLimit = .2f;
    [SerializeField] private float _attackCooldown = .1f;
    [SerializeField] private float _maxKeepComboTime = .5f;
    [SerializeField] private float[] _attackHitTime;
    [SerializeField] private float _attackDistance = 3;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _adventurer = GetComponent<Adventurer>();
        _rb = GetComponent<Rigidbody2D>();
        _originalMoveSpeed = _adventurer._moveSpeed;
    }

    private void FixedUpdate()
    {
        //If player just finished the attack animation
        if(_adventurer._isAttacking && !AnimatorIsPlaying("Adventurer_Attack_" + _attackCounter))
        {
            _adventurer._isAttacking = false;
            _attackEnded = true;
            _lastAttackTime = Time.time;

            //Finished combo (reset)
            if (_attackCounter == 3) _attackCounter = 0;

            //If player pressed attack button a bit earlier, still consider the attack
            if (Time.time <= _lastAttackAttmeptTime + _preAttackTimeLimit) OnAttack();
            else _adventurer._moveSpeed = _originalMoveSpeed;
        }

        //Reset the attack combo if player takes to long to attack again
        if(_attackEnded && Time.time > _lastAttackTime + _maxKeepComboTime)
        {
            _attackCounter = 0;
            _attackEnded = false;
        }
    }

    void OnAttack()
    {
        _lastAttackAttmeptTime = Time.time;

        if (!_adventurer._isAttacking && !_adventurer._isSliding && !_adventurer._canWallJump && _adventurer._canJump)
        {
            _adventurer._isAttacking = true;
            _attackEnded = false;

            //Stop movement while attacking
            _adventurer._moveSpeed = 0;
            _rb.linearVelocityX = 0;

            _animator.SetInteger(Constants.ANIM_ATTACK_COUNTER, ++_attackCounter);
            _animator.SetTrigger(Constants.ANIM_ATTACK);


        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(_attackCounter-1);

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