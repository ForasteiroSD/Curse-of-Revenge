using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utils;
using Unity.VisualScripting;
using Unity.Mathematics;

public class Adventurer : MonoBehaviour
{
    //Player Variables
    [SerializeField] private float _life = 20;
    public bool _isDead { get; private set; } = false;
    private CapsuleCollider2D _playerCollider;

    //Controls
    [SerializeField] private float _analogDeadZone = .3f;

    //Components
    private Rigidbody2D _rb;
    private Animator _animator;

    //Movement
    private Vector2 _moveDirection;
    public bool _canMove { get; set; } = true;
    [SerializeField] public float _originalMoveSpeed { get; set; } = 5f;
    [SerializeField] public float _moveSpeed { get; set; } = 5f;

    //Jump
    private bool _releasedJumpButton = false;
    private float _gravityScale;
    private float _fallGravityScale;
    private float _jumpHangGravityMult;
    public bool _considerPreJump { get; set; } = true;
    public bool _canJump { get; set; } = true;
    public float _lastJumpTime { get; private set; } = -10;
    public float _originalFallingSpeed { get; private set; }
    public bool _isJumping { get; set; } = false;
    [SerializeField] public float _preJumpTimeLimit { get; private set; } = .3f;
    [SerializeField] private float _fallGravityScaleMultiplier = 3f;
    [SerializeField] private float _jumpHeight = 2.5f;
    [SerializeField] public float _maxFallingSpeed { get; set; } = 8f;
    [SerializeField] public float _acceptJumpTime { get; private set; } = .11f;

    //Wall slide
    private bool _isWallJumping = false;
    public bool _canWallJump { get; set; } = false;
    [SerializeField] public float _wallMaxFallingSpeed = 1;
    [SerializeField] private float _stopTimeAfterWallJump = .7f;
    [SerializeField] private float _velocityDecreaserAfterWallJump = .4f;

    //Slide
    private float _roolStartVelocity;
    private float _slideVelocityDecreaser = .03f;
    private float _lastSlideTime = 0;
    private float _stopSlideVelocity = 1;
    public bool _isSliding { get; private set; } = false;
    public float _lastSlideAttemptTime { get; private set; } = -10;
    [SerializeField] public float _preSlideTimeLimit { get; private set; } = .3f;
    [SerializeField] private float _slideForce = 3f;
    [SerializeField] private float _slideCooldown = 1f;

    //Attack
    public bool _isAttacking { get; set; } = false;

    //GetHit
    public bool _isGettingHit { get; private set; } = false;

    //Camera Control
    private CameraController _cameraController; 

    //Platform
    private GameObject _platform;
    [SerializeField] private float _timeToFallThroughPlatform = .25f;

    //Special Attack
    private float _lastSpecialAttackTime = -10;
    public bool _isUsingSpecialAttack { get; private set; } = false;
    [SerializeField] private float _specialAttackCooldown = 10f;
    [SerializeField] private GameObject _specialAttack;

    private PauseScript _pause;

    void Awake()
    {
        //Get Components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerCollider = GetComponent<CapsuleCollider2D>();
        
        //Set gravity scales
        _gravityScale = _rb.gravityScale;
        _fallGravityScale = _gravityScale * _fallGravityScaleMultiplier;
        _rb.gravityScale = _fallGravityScale;

        //Set original variables values
        _originalFallingSpeed = _maxFallingSpeed;
        _originalMoveSpeed = _moveSpeed;

        //Get Hierarchy Elements
        _cameraController = FindFirstObjectByType<CameraController>();
        _pause = FindFirstObjectByType<PauseScript>();
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

        if(_isSliding)
        {
            //If player is slow enough, stop sliding
            if (Mathf.Sign(_rb.linearVelocityX) != Mathf.Sign(transform.localScale.x) || _rb.linearVelocityX == 0 || _canMove)
            {
                _isSliding = false;
                _canMove = true;
                _lastSlideTime = Time.time;
                _animator.SetBool(Constants.ANIM_IS_SLIDING, false);
            }

            //Else decreases player velocity (in order to stop the slide)
            else _rb.linearVelocityX = _rb.linearVelocityX + (_roolStartVelocity * _slideVelocityDecreaser * -Mathf.Sign(transform.localScale.x));
        }

        //Stop sliding animation
        if(Mathf.Abs(_rb.linearVelocityX) <= _stopSlideVelocity) _animator.SetBool(Constants.ANIM_IS_SLIDING, false);

        //Set _isGettingHit to false if hit animation already ended
        if (_isGettingHit && !AnimatorIsPlaying("Adventurer_Hurt")) _isGettingHit = false;

        //Set _isUsingSpecialAttack to false if hit animation already ended
        if (_isUsingSpecialAttack && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Adventurer_Special_Attack")) _isUsingSpecialAttack = false;
    }

    public void OnPause(InputValue inputValue)
    {
        if (inputValue.isPressed)
        { 
            _pause.PauseGame();
        }
        
    }
    
    void OnMove(InputValue inputValue)
    {
        _moveDirection = inputValue.Get<Vector2>();

        //Call the function to handle the camera response to the momevent
        if(_cameraController && !_isDead) _cameraController.Moved(_moveDirection.x);

        //Check X analog deadzone
        if (_moveDirection.x > _analogDeadZone) _moveDirection.x = 1;
        else if (_moveDirection.x < -_analogDeadZone) _moveDirection.x = -1;
        else _moveDirection.x = 0;

        //Check Y analog deadzone
        if (_moveDirection.y > _analogDeadZone) _moveDirection.y = 1;
        else if (_moveDirection.y < -_analogDeadZone) _moveDirection.y = -1;
        else _moveDirection.y = 0;
    }

    public void OnJump()
    {
        if(_considerPreJump) _lastJumpTime = Time.time;
        else _considerPreJump = true;

        if (((_canJump && !_isSliding) || _canWallJump) && !_isGettingHit && !_isUsingSpecialAttack && !_isDead)
        {
            _isJumping = true;

            //If player is on platform
            if(_moveDirection.y < 0 && _platform != null) {
                StartCoroutine(FallThroughPlatform());
                return;
            }

            //If player jump while attacking, moveSpeed would be set to 0. So, set it back to the original value
            _moveSpeed = _originalMoveSpeed;

            _releasedJumpButton = false;
            _rb.gravityScale = _gravityScale; //Set the normal gravity scale (not the falling one)
            _animator.SetTrigger(Constants.ANIM_JUMP);

            _rb.linearVelocityY = 0; //Set Y velocity to 0, in case the player is already falling when jump
            float _jumpForce = (float)Mathf.Sqrt(_jumpHeight * (Physics2D.gravity.y * _rb.gravityScale) * -2) * _rb.mass;
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _canJump = false;

            //If is wall jumping, besides normal jump, does a few more things
            if (_canWallJump) StartCoroutine(WallJump(_jumpForce));

            //After jump, decreases _lastJumpTime to avoid unwanted pre jumps
            _lastJumpTime -= _preJumpTimeLimit;
        }
    }

    void OnStopJump()
    {
        _releasedJumpButton = true;

        //Decreases _lastJumpTime to avoid unwanted pre jumps
        _lastJumpTime -= _preJumpTimeLimit;
    }

    public void OnSlide()
    {
        _lastSlideAttemptTime = Time.time;

        //If can slide
        if (Mathf.Abs(_moveDirection.x) > 0 && _canJump && !_isSliding && (_lastSlideTime + _slideCooldown <= Time.time) && !_isAttacking && !_isGettingHit && !_isUsingSpecialAttack && !_isDead)
        {
            _canMove = false;
            _isSliding = true;
            _rb.AddForce(new Vector2(_moveDirection.x, 0) * _slideForce, ForceMode2D.Impulse);
            _roolStartVelocity = Mathf.Abs(_rb.linearVelocityX);
            _animator.SetBool(Constants.ANIM_IS_SLIDING, true);
        }
    }

    private void OnSpecialAttack() {
        if(Time.time >= _lastSpecialAttackTime + _specialAttackCooldown && !_isJumping && !_isDead && !_isAttacking && !_isGettingHit && !_isSliding && !_isUsingSpecialAttack) {
            _lastSpecialAttackTime = Time.time;
            _rb.linearVelocityX = 0;
            _isUsingSpecialAttack = true;
            _animator.SetTrigger(Constants.ANIM_SPECIAL_ATTACK);
        }
    }

    void MovePlayer()
    {
        if(_canMove && !_isWallJumping && !_isUsingSpecialAttack && !_isDead)
        {
            Vector3 scale = transform.localScale;
            bool isRunning = Mathf.Abs(_moveDirection.x) > Mathf.Epsilon && !_isAttacking;

            _rb.linearVelocityX = _moveDirection.x * _moveSpeed * Convert.ToInt32(!_isGettingHit);
            if (isRunning)
            {
                //Look to the right direction and set animation of running
                transform.localScale = new Vector3(Mathf.Sign(_moveDirection.x) * Mathf.Abs(scale.x), scale.y, scale.z);
                _animator.SetBool(Constants.ANIM_IS_RUNNING, true);
            }
            else _animator.SetBool(Constants.ANIM_IS_RUNNING, false);
        }

        else if(_isWallJumping)
        {
            //If is going up, smoothly decreases x velocity (to make the path to the wall smoother)
            if (!(Mathf.Sign(_moveDirection.x) == Mathf.Sign(transform.localScale.x))) _rb.linearVelocityX += _moveDirection.x * _velocityDecreaserAfterWallJump;
        }
    }

    public void GetHit(float damage)
    {
        _life = Mathf.Max(0, _life-damage);
        print("My life: " + _life);
        
        if(_life > 0)
        {
            _isGettingHit = true;
            if(!_isSliding && !_canWallJump) _animator.SetTrigger(Constants.ANIM_GET_HIT);
        }
        else if(!_isDead)
        {
            _isDead = true;
            StartCoroutine(Die());
        }
    }

    private void CastSpecialAttack() {
        Destroy(Instantiate(_specialAttack, transform.Find("SpecialAttack Position").transform.position, Quaternion.identity), 10);
    }

    IEnumerator Die()
    {
        // GetComponentInParent<PlayerInput>().enabled = false;
        _canMove = false;
        _canJump = false;
        _canWallJump = false;
        _rb.linearVelocityX = 0;
        _maxFallingSpeed = _originalFallingSpeed;
        _animator.SetTrigger(Constants.ANIM_DIE);

        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator WallJump(float jumpForce)
    {
        bool isGoingUp = Mathf.Sign(_moveDirection.x) == Mathf.Sign(transform.localScale.x);

        Vector3 scale = transform.localScale;
        if (_moveDirection.x == 0 || isGoingUp) jumpForce = jumpForce / 1.5f; //If is going up or is not moving, decreases the horizontal jump force
        if (isGoingUp) _rb.AddForce(Vector2.up * jumpForce / 3, ForceMode2D.Impulse); //If is going up, give a little extra force to the vertical jump
        _isWallJumping = true;

        _rb.AddForce(Vector2.right * -Mathf.Sign(transform.localScale.x) * jumpForce, ForceMode2D.Impulse);
        transform.localScale = new Vector3(-scale.x, scale.y, scale.z);

        yield return new WaitForSeconds(_stopTimeAfterWallJump);
        _isWallJumping = false;
    }

    IEnumerator FallThroughPlatform() {
        BoxCollider2D platformCollider = _platform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(_playerCollider, platformCollider);
        yield return new WaitForSeconds(_timeToFallThroughPlatform);
        Physics2D.IgnoreCollision(_playerCollider, platformCollider, false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag(Constants.TAG_PLATFORM)) _platform = other.gameObject;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag(Constants.TAG_PLATFORM)) _platform = null;
    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}