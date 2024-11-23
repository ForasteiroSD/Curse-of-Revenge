using System.Collections;
using UnityEngine;
using Utils;

public class KingScript : EnemiesScript
{
    CapsuleCollider2D _collider;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        SFXManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected override void Update()
    {

        if (_attackedOnBorder && _isAttacking == 1)
        {
            StartCoroutine(Idle());
            _attackedOnBorder = false;
            return;
        }
        
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

    protected override IEnumerator Hit()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("KingAttack1")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
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