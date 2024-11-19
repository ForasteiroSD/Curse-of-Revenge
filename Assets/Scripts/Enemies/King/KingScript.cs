using System.Collections;
using UnityEngine;
using Utils;

public class KingScript : EnemiesScript
{
    CapsuleCollider2D _collider;

    [SerializeField] float _attack2Cooldown;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected override void Update()
    {

        if (_isAttacking == 0)
        {
            float distance = _player.transform.position.x - transform.position.x;

            if(distance >= 0 && _horSpeed < 0) Flip();
            else if(distance < 0 && _horSpeed > 0) Flip();
            return;
        }

        if (_hit || _death) return;

        if (_isChasing)
        {
            Chase();
        }
        else
        {
            Move();
        }
    }

    protected override IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        string anim;
        float attackCooldown;

        if(Random.Range(0, 2) == 1)
        {
            anim = "Attack2";
            attackCooldown = _attack2Cooldown;
        } else
        {
            anim = "Attack1";
            attackCooldown = _attackCooldown;
        }

        _animator.SetTrigger(anim);
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(attackCooldown);

        _isAttacking = 1;

    }

    protected override IEnumerator Hit()
    {
        _hit = true;
        _animator.SetBool(Constants.IDLE_ENEMY, true);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    protected override IEnumerator Death()
    {
        _death = true;
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(Constants.REVENGE_POINT_DROP_TIME);

        DropRevengePoint();

        Destroy(transform.parent.gameObject, 5);
    }

    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}