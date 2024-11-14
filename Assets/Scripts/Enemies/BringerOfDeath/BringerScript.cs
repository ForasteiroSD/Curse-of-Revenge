using System.Collections;
using System.Data.Common;
using TMPro;
using UnityEngine;
using Utils;

public class BringerScript : BossScript
{
    //Referencies
    Animator _animator;
    Rigidbody2D _rb;
    CapsuleCollider2D _collider;
    [SerializeField] Transform _minPos;
    [SerializeField] Transform _maxPos;
    [SerializeField] Transform _bringerPos;
    [SerializeField] private GameObject _revengePoint;
    [SerializeField] GameObject _textDamage;
    Transform _player;
    
    //Status
    [SerializeField] int _valuePerRevengePoint = 1;
    [SerializeField] int _revengePointsQuantity = 1;

    [SerializeField] float _horSpeed = 2f;
    [SerializeField] float _chaseSpeedMultiplier = 1.1f;

    [SerializeField] float _attackDistance = 1.5f;
    [SerializeField] float _attackCooldown = 1.5f;
    [SerializeField] float _spellCooldown = 5f;
    [SerializeField] float _teleportDelay = 1f;
    [SerializeField] float _minDist = 6;

    [SerializeField] float _damageReceivedMult = 0.8f;
    [SerializeField] float _health = 20f;
    [SerializeField] float _hitDelay = 1f;
    [SerializeField] float[] _attackDamages;
    [SerializeField] float _dodgeChange = 0.7f;
    [SerializeField] GameObject[] _spellsTypes;
    [SerializeField] GameObject _spellSequencial;
    [SerializeField] float _delayOnSequencialSpells;
    float _minTimeAttack = 0.3f;
    float _lastAttackTime;
    float _spellAnimatorSpeed = 1;
    [SerializeField] float _spellAnimatorSpeedAcelerator = 0.05f;
    [SerializeField] float _timeToSpeedUpSpells = 8;

    //States
    bool _idle = false;
    int _isAttacking = 1;
    bool _death = false;
    bool _canUseSpell = true;
    bool _teleport = false;
    int _currentAttack = 0;
    bool _isUsingAttack1 = false;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        StartCoroutine(UpdateSpellSpeed());
    }

    IEnumerator UpdateSpellSpeed() 
    {
        yield return new WaitForSecondsRealtime(_timeToSpeedUpSpells);
        _spellAnimatorSpeed += _spellAnimatorSpeedAcelerator;
        StartCoroutine(UpdateSpellSpeed());
    }

    void Update()
    {
        if (_isUsingAttack1)
        {
            float distance = _player.transform.position.x - _bringerPos.position.x;

            if(distance >= 0 && _horSpeed < 0) Flip();
            else if(distance < 0 && _horSpeed > 0) Flip();
            return;
        }

        if (_idle || _isAttacking == 0 || _teleport) return;

        Chase();
    }

    void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
    }

    void Chase()
    {
        if (_canUseSpell)
        {
            _canUseSpell = false;
            StartCoroutine(Spell());
            return;
        }

        float distance = _player.transform.position.x - _bringerPos.position.x;

        if (distance >= 0 && _horSpeed < 0) Flip();
        else if (distance < 0 && _horSpeed > 0) Flip();


        if (Mathf.Abs(distance) < _attackDistance) Attack();
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

        damage = Mathf.Ceil(damage * _damageReceivedMult);
        
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

    void Attack()
    {
        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        _idle = true;
        _teleport = false;

        _isUsingAttack1 = true;
        _currentAttack = 1;
        _animator.SetTrigger("Attack");
    }

    //called by attack animation
    void StopAttack1()
    {
        _isUsingAttack1 = false;
    }

    IEnumerator Spell()
    {
        _isAttacking = 0;
        _currentAttack = 2;
        _idle = true;
        GameObject spell;

        _animator.SetTrigger("Cast");

        int attackNumber = Random.Range(0, _spellsTypes.Length+2);

        //sequencial from left
        if (attackNumber == _spellsTypes.Length)
        {
            Vector3 pos = _minPos.position;
            pos.x += 1f;
            float maxPos = _maxPos.position.x;
            spell = Instantiate(_spellSequencial, pos, Quaternion.identity);
            float animTIme = spell.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
            Destroy(spell, 5);
            pos.x += 2f;

            while (pos.x < maxPos)
            {
                yield return new WaitForSecondsRealtime(_delayOnSequencialSpells);
                Destroy(Instantiate(_spellSequencial, pos, Quaternion.identity), 3);
                pos.x += 2f;
            }
            yield return new WaitForSecondsRealtime(_delayOnSequencialSpells);
            Destroy(Instantiate(_spellSequencial, pos, Quaternion.identity), 3);
            yield return new WaitForSecondsRealtime(animTIme);
            StopAttack();
            yield break;
        }
        //sequencial from right
        else if (attackNumber == _spellsTypes.Length + 1)
        {
            Vector3 pos = _maxPos.position;
            pos.x -= 1f;
            float minPos = _minPos.position.x;
            spell = Instantiate(_spellSequencial, pos, Quaternion.identity);
            float animTIme = spell.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
            Destroy(spell, 5);
            pos.x -= 2f;

            while(pos.x > minPos) {
                yield return new WaitForSecondsRealtime(_delayOnSequencialSpells);
                Destroy(Instantiate(_spellSequencial, pos, Quaternion.identity), 3);
                pos.x -= 2f;
            }
            yield return new WaitForSecondsRealtime(_delayOnSequencialSpells);
            Destroy(Instantiate(_spellSequencial, pos, Quaternion.identity), 3);
            yield return new WaitForSecondsRealtime(animTIme);
            StopAttack();
            yield break;
        }
        //big spell
        else if (attackNumber == 1)
        {
            float distMin = _player.position.x - _minPos.position.x;
            float distMax = _maxPos.position.x - _player.position.x;

            spell = Instantiate(_spellsTypes[attackNumber], _player.transform.position, Quaternion.identity);
            if (distMin > distMax)
            {
                spell.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                spell.transform.localScale = new Vector3(spell.transform.localScale.x, spell.transform.localScale.y*(-1), spell.transform.localScale.z);
            }
        }
        //double spell
        else if (attackNumber == 3)
        {
            spell = Instantiate(_spellsTypes[attackNumber], _player.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)));
        }
        else
        {
            spell = Instantiate(_spellsTypes[attackNumber], _player.transform.position, Quaternion.identity);
        }

        foreach (var animator in spell.GetComponentsInChildren<Animator>())
        {
            animator.speed = _spellAnimatorSpeed;
        }

        Destroy(spell, 3);
    }

    //called by event on cast attack animation
    void StopSpellCast()
    {
        _animator.SetBool("Idle", true);
        StartCoroutine(SpellCooldown());
    }

    //called by spell attack script
    public void StopSpell()
    {
        StopAttack();
    }

    IEnumerator SpellCooldown()
    {
        yield return new WaitForSecondsRealtime(_spellCooldown);
        _canUseSpell = true;
    }

    //called by event on attack animation
    void StopAttack()
    {
        _isAttacking = 1;
        _currentAttack = 0;

        StartCoroutine(IdleTimeout(_attackCooldown));
    }

    //called by input system
    void OnAttack()
    {
        if (_isAttacking == 0 || _teleport || !_idle) return;

        Vector2 distance = _player.transform.position - _bringerPos.position;
        if (Mathf.Abs(distance.x) < _attackDistance) {
            if (Random.Range(0f, 1f) <= _dodgeChange) {
                _idle = true;
                _teleport = true;
                _animator.SetTrigger("Teleport");
            }
        }
    }

    //called by event on teleport animation
    void Teleport()
    {
        float newPos;
        float distMin = _player.position.x - _minPos.position.x;
        float distMax = _maxPos.position.x - _player.position.x;

        if (distMin > distMax)
        {
            newPos = Random.Range(_minPos.position.x + 1f, _player.position.x - _minDist);
        }
        else 
        {
            newPos = Random.Range(_player.position.x + _minDist, _maxPos.position.x - 1f);
        }

        transform.position = new Vector3(newPos, transform.position.y, transform.position.z);

    }

    //called by event on teleport reverse animation
    void StopTeleport() 
    {
        _teleport = false;
        StartCoroutine(IdleTimeout(_teleportDelay));
    }

    IEnumerator IdleTimeout(float delay)
    {
        _animator.SetBool("Idle", true);

        yield return new WaitForSecondsRealtime(delay);

        if (_isAttacking == 0) yield break;
        
        _idle = false;

    }

    public void GiveDamage()
    {
        if(_currentAttack == 2 || Time.time >= _lastAttackTime + _minTimeAttack)
        {
            _player.gameObject.GetComponent<Adventurer>().GetHit(_attackDamages[_currentAttack-1]);
            _lastAttackTime = Time.time;
        }
    }

    IEnumerator Hit()
    {
        if (_isAttacking == 0 || _teleport) yield break;

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
