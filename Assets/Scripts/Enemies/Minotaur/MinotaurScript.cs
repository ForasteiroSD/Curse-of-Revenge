using System.Collections;
using UnityEngine;
using Utils;

public class MinotaurScript : EnemiesScript
{
    CapsuleCollider2D _collider;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        SFXManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        indexSFX = 12;
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected override IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        SFXManager.TocarSFX(indexSFX);

        if(Random.Range(0, 2) == 1)
        {
            _animator.SetTrigger("Attack 2");
        } else
        {
            _animator.SetTrigger("Attack 1");
        }

        _animator.SetBool(Constants.IDLE_ENEMY, true);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    protected override IEnumerator Hit()
    {
        string animation = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (animation.CompareTo("Minotaur Attack 1") == 0 || animation.CompareTo("Minotaur Attack 2") == 0) yield break;
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
