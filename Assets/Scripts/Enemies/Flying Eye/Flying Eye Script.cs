using System.Collections;
using UnityEngine;
using Utils;

public class FlyingEyeScript : EnemiesScript
{
    CircleCollider2D _collider;

    [SerializeField] float _verSpeed = 2f;

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _flyHeightPos;
    [SerializeField] float _unitsToMove;

    float _startPos;
    float _endPos;
    bool _goingUp = false;
    bool _goingDown = false;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _player = FindFirstObjectByType<Adventurer>().transform;
        _startPos = transform.position.x;
        _endPos = _startPos + _unitsToMove;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (_death) return;

        if (_goingUp)
        {
            _rb.linearVelocityY = _verSpeed;

            Vector3 direction = new Vector3(1, 0, 0);
            if (_horSpeed < 0) direction = new Vector3(-1, 0, 0);

            RaycastHit2D hit = Physics2D.Raycast(_flyHeightPos.position, direction, 1.5f, _groundLayer);

            if (hit.collider == null)
            {
                _rb.linearVelocityY = 0.5f;
                _goingUp = false;
            }

        } else if (_goingDown)
        {
            _rb.linearVelocityY = -_verSpeed;

            Vector3 direction = new Vector3(0, -1, 0);

            RaycastHit2D hit = Physics2D.Raycast(_flyHeightPos.position, direction, 0.1f, _groundLayer);

            if (hit.collider != null)
            {
                _rb.linearVelocityY = 0f;
                _goingDown = false;
            }
        }
        else
        {
            _rb.linearVelocityY = 0;

            if (_isAttacking == 0 || _hit) return;

            if (_isChasing)
            {
                Chase();
            }
            else
            {
                Move();
            }
        }
    }
    protected override void Move()
    {
        if(_idle) return;

        if (_horSpeed > 1)
        {
            if (_rb.position.x > _endPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        else
        {
            if (_rb.position.x < _startPos && !_isChasing)
            {
                StartCoroutine(Idle());
                return;
            }
        }

        _rb.linearVelocityX = _horSpeed;
    }

    public override void PlayerInRange()
    {
        if(_goingDown || _goingUp) return;

        if (_player.position.x > _minChasePos.position.x && _player.position.x < _maxChasePos.position.x)
        {
            _isChasing = true;
        }
        else
        {
            _rb.linearVelocityX = 0;
            _isChasing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_goingDown) return;
        _isChasing = false;
        _rb.linearVelocityX = 0;
        _rb.linearVelocityY = 0;
        Vector3 direction = new Vector3(1, 0, 0);
        if (_horSpeed < 0) direction = new Vector3(-1, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1f), direction, 1.5f, _groundLayer);

        //if there's a wall in front and it's height is greater then height the eye can go up
        if (hit.collider != null)
        {
            StartCoroutine(Idle());
        }
        else
        {
            _goingUp = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _rb.linearVelocityY = 0;
    }

    public void GoDown()
    {
        if (!_goingUp)
        {
            _rb.linearVelocityX = 0;

            Vector3 direction = new Vector3(0, -1, 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3f, _groundLayer);

            //if there's ground bellow the eye and it's not too deep
            if (hit.collider != null)
            {
                _goingDown = true;
            }
            else
            {
                StartCoroutine(Idle());
            }
        }
    }

    protected override void Chase()
    {
        if (_idle)
        {
            _idle = false;
        }

        float distance = _player.transform.position.x - transform.position.x;

        if (distance > 0)
        {
            if (_horSpeed < 0)
            {
                Flip();
            }
        }
        else
        {
            if (_horSpeed > 0)
            {
                Flip();
            }
        }

        if (Mathf.Abs(distance) < _attackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            _rb.linearVelocityX = _horSpeed * _chaseSpeedMultiplier * _isAttacking;
        }
    }

    protected override IEnumerator Attack()
    {
        if (_death) yield break;

        //get into attack mode
        _rb.linearVelocityX = 0;
        _isAttacking = 0;
        _animator.SetTrigger(Constants.ATTACK_ENEMY);

        //wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = 1;

    }

    protected override IEnumerator Idle()
    {
        _rb.linearVelocityX = 0;
        _idle = true;

        yield return new WaitForSeconds(_idleTime);

        if (_isChasing) yield break;

        _idle = false;
        Flip();
    }

    protected override IEnumerator Hit()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy Attack")) yield break;
        _hit = true;
        _animator.SetTrigger(Constants.HIT_ENEMY);
        _rb.linearVelocityX = 0;

        yield return new WaitForSeconds(_hitDelay);

        _hit = false;
    }

    protected override IEnumerator Death()
    {
        _death = true;
        _collider.offset = new Vector2(_collider.offset.x, -0.15f);
        _animator.SetTrigger(Constants.DEATH_ENEMY);
        _rb.linearVelocityX = 0;
        _rb.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(Constants.REVENGE_POINT_DROP_TIME);

        DropRevengePoint();

        Destroy(transform.parent.gameObject, 3);
    }

    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}
