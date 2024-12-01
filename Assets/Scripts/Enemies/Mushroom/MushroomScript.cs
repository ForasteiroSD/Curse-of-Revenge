using System.Collections;
using UnityEngine;
using Utils;

public class MushroomScript : EnemiesScript
{
    BoxCollider2D _collider;
    [SerializeField] GameObject _projectile;

    [SerializeField] Transform _attackPos;
    [SerializeField] float _instatiateDelay;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        indexSFX = 24;
        SFXManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _collider = GetComponent<BoxCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
    }

    protected override IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        SFXManager.TocarSFX(indexSFX);
        _isAttacking = 0;
        _animator.SetTrigger(Constants.ATTACK_ENEMY);
        _animator.SetBool(Constants.IDLE_ENEMY, true);

        yield return new WaitForSeconds(_instatiateDelay);

        if(_death) yield break;

        GameObject project = Instantiate(_projectile, _attackPos.position, Quaternion.identity);
        project.GetComponent<Projectile>().damage = _attackDamage;

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}
