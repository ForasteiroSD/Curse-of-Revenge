using Unity.VisualScripting;
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
    private GameObject[] _targets;
    [SerializeField] private float _preAttackTimeLimit = .2f;
    [SerializeField] private float _attackCooldown = .1f;
    [SerializeField] private float _maxKeepComboTime = .5f;
    [SerializeField] private float _attackDamage;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
        _adventurer = GetComponentInParent<Adventurer>();
        _rb = GetComponentInParent<Rigidbody2D>();
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
            else _adventurer._moveSpeed = _adventurer._originalMoveSpeed;
        }

        if(!_adventurer._isAttacking) _adventurer._moveSpeed = _adventurer._originalMoveSpeed;

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

        if (!_adventurer._isAttacking && !_adventurer._isSliding && !_adventurer._canWallJump && !_adventurer._isJumping && !_adventurer._isGettingHit && !_adventurer._pause.isPaused && !_adventurer._isDead)
        {
            _adventurer.SFXManager.TocarSFX(0);
            _adventurer._isAttacking = true;
            _attackEnded = false;

            //Stop movement while attacking
            _adventurer._moveSpeed = 0;
            _rb.linearVelocityX = 0;

            _animator.SetInteger(Constants.ANIM_ATTACK_COUNTER, ++_attackCounter);
            _animator.SetTrigger(Constants.ANIM_ATTACK);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.TAG_ENEMY))
        {
            EnemiesScript script = collision.gameObject.GetComponent<EnemiesScript>();
            if(script)
            {
                if(_attackCounter < 3) script.GetHit(_attackDamage);
                else  script.GetHit(_attackDamage + 2);
            } else
            {
                if(_attackCounter < 3) collision.gameObject.GetComponent<BossScript>().GetHit(_attackDamage);
                else collision.gameObject.GetComponent<BossScript>().GetHit(_attackDamage + 2);
            }
            
        }
        else if(collision.CompareTag(Constants.TAG_PROJECTILE))
        {
            StartCoroutine(collision.gameObject.GetComponent<Projectile>().DestroyProjectile(0));
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