using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Adventurer : MonoBehaviour
{
    //Controls
    [SerializeField] private float _analogDeadZone = .3f;

    //Components
    private Rigidbody2D _rb;
    private Animator _animator;

    //Movement
    private Vector2 _moveDirection;
    [SerializeField] private float _moveSpeed = 5f;

    //Jump
    public bool _canJump { get; set; } = true;
    private bool _releasedJumpButton = false;
    private float _gravityScale;
    private float _fallGravityScale;
    private float _jumpHangGravityMult;
    public float _lastJumpTime { get; private set; } = -10;
    [SerializeField] public float _preJumpTimeLimit { get; private set; } = .3f;
    [SerializeField] private float _fallGravityScaleMultiplier = 3f;
    [SerializeField] private float _jumpHeight = 2.5f;
    [SerializeField] public float _maxFallingSpeed { get; set; } = 8f;
    [SerializeField] public float _acceptJumpTime { get; private set; } = .2f;

    //Wall slide
    [SerializeField] public float _wallMaxFallingSpeed = 1;


    void Awake()
    {
        //Get Components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        //Set gravity scales
        _gravityScale = _rb.gravityScale;
        _fallGravityScale = _gravityScale * _fallGravityScaleMultiplier;
        _rb.gravityScale = _fallGravityScale;
    }

    void FixedUpdate()
    {
        MovePlayer();

        //Stop jump if player releases the jump button
        if (_releasedJumpButton) _rb.linearVelocityY = _rb.linearVelocityY * 0.8f;

        //If player is going up, slowly deacreases y velocity to make it feel better
        if (_rb.linearVelocityY > 0) _rb.linearVelocityY *= 0.98f;

        //If player is falling, increases the gravity, checing the max falling velocity
        if (_rb.linearVelocityY < 0)
        {
            _releasedJumpButton = false;
            _rb.gravityScale = _fallGravityScale;
            _rb.linearVelocityY = Mathf.Max(_rb.linearVelocityY, -_maxFallingSpeed);
        }
    }

    void OnMove(InputValue inputValue)
    {
        _moveDirection = inputValue.Get<Vector2>();

        //Check X analog deadzone
        if (_moveDirection.x > _analogDeadZone) _moveDirection.x = 1;
        else if (_moveDirection.x < -_analogDeadZone) _moveDirection.x = -1;
        else _moveDirection.x = 0;

        //Check Y analog deadzone
        if (_moveDirection.y > _analogDeadZone) _moveDirection.y = 1;
        else if (_moveDirection.y < -_analogDeadZone) _moveDirection.y = -1;
        else _moveDirection.y = 0;
    }

    void MovePlayer()
    {
        Vector3 scale = transform.localScale;
        bool isRunning = Mathf.Abs(_moveDirection.x) > Mathf.Epsilon;

        _rb.linearVelocityX = _moveDirection.x * _moveSpeed;
        if (isRunning)
        {
            transform.localScale = new Vector3(Mathf.Sign(_moveDirection.x) * Mathf.Abs(scale.x), scale.y, scale.z);
            _animator.SetBool(Constants.ANIM_IS_RUNNING, true);
        }
        else _animator.SetBool(Constants.ANIM_IS_RUNNING, false);
    }

    public void OnJump()
    {
        _lastJumpTime = Time.time;

        if (_canJump)
        {
            _releasedJumpButton = false;
            _rb.gravityScale = _gravityScale; //Set the normal gravity scale (not the falling one)
            _animator.SetTrigger(Constants.ANIM_JUMP);

            _rb.linearVelocityY = 0; //Set Y velocity to 0, in case the player is already falling when jump
            float _jumpForce = (float)Mathf.Sqrt(_jumpHeight * (Physics2D.gravity.y * _rb.gravityScale) * -2) * _rb.mass;
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _canJump = false;
        }
    }

    void OnStopJump()
    {
        _releasedJumpButton = true;
    }
}