using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utils;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class Adventurer : MonoBehaviour
{
    //Management
    private GameManager _gameManager;
    private Animator _ascendingAnimator;

    //Player Variables
    public float life;
    public AudioManager SFXManager { get; set; }
    public float _maxLife { get; private set; } = 3;
    private CapsuleCollider2D _playerCollider;
    public bool _isDead { get; private set; } = false;

    //Controls
    [SerializeField] private float _analogDeadZone = .3f;

    //Components
    private Rigidbody2D _rb;
    private Animator _animator;

    //Movement
    private Vector2 _moveDirection;
    public bool _canMove { get; set; } = true;
    [SerializeField] public float _originalMoveSpeed { get; set; } = 5f;
    public float _moveSpeed = 5f;

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
    private Image _specialAttackBackgroundUI;
    private Image _specialAttackIconUI;
    private Image _specialAttackCommandUI;
    private TextMeshProUGUI _specialAttackTextUI;
    private bool _canUseSpecialAttack = true;
    public bool _isUsingSpecialAttack { get; private set; } = false;
    [SerializeField] private int _specialAttackCooldown = 10;
    [SerializeField] private GameObject _specialAttack;

    //Heal
    private int _maxHealPotions;
    private int _healPotionsLeft;
    private bool _isHealing = false;
    private Image _healPotionBackgroundUI;
    private Image _healPotionIconUI;
    private Image _healPotionCommandUI;
    private Animator _healPotionAnimator;
    private int _haelLifeAmount = 5;

    //Stats
    private int _lifeUpgradeLevel;
    private int _healBottlesUpgradeLevel;
    private int _healAmountUpgradeLevel;
    private int _specialCooldownUpgradeLevel;
    public bool _specialAttackUnlocked { get; private set; }
    public bool _slideUnlocked { get; private set; }
    [SerializeField] private int _lifeSizeIncreasePerPoint = 40;

    public PauseScript _pause { get; private set; }

    void Awake()
    {
        //Get Elements
        SFXManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _gameManager = FindFirstObjectByType<GameManager>();

        //Get UI Elements
        GameObject colldownsUI = GameObject.Find("Cooldowns");
        GameObject specialAttackUI = colldownsUI.transform.Find("SpecialAttack").gameObject;
        GameObject healPotionUI = colldownsUI.transform.Find("HealPotion").gameObject;

        _specialAttackBackgroundUI = specialAttackUI.transform.Find("Background").GetComponent<Image>();
        _specialAttackIconUI = specialAttackUI.transform.Find("Icon").GetComponent<Image>();
        _specialAttackCommandUI = specialAttackUI.transform.Find("Control").GetComponent<Image>();
        _specialAttackTextUI = specialAttackUI.transform.Find("Time").GetComponent<TextMeshProUGUI>();

        _healPotionBackgroundUI = healPotionUI.transform.Find("Background").GetComponent<Image>();
        _healPotionIconUI = healPotionUI.transform.Find("Icon").GetComponent<Image>();
        _healPotionCommandUI = healPotionUI.transform.Find("Control").GetComponent<Image>();
        _healPotionAnimator = healPotionUI.transform.Find("Icon").GetComponent<Animator>();

        //Get Components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerCollider = GetComponent<CapsuleCollider2D>();
        _ascendingAnimator = GameObject.Find("AscendBackGround").GetComponent<Animator>();
        
        //Set gravity scales
        _gravityScale = _rb.gravityScale;
        _fallGravityScale = _gravityScale * _fallGravityScaleMultiplier;
        _rb.gravityScale = _fallGravityScale;

        //Getting stats
        _lifeUpgradeLevel = _gameManager._lifeUpgradeLevel;
        _healBottlesUpgradeLevel = _gameManager._healBottlesUpgradeLevel;
        _healAmountUpgradeLevel = _gameManager._healAmountUpgradeLevel;
        _specialCooldownUpgradeLevel = _gameManager._specialCooldownUpgradeLevel;
        _specialAttackUnlocked = _gameManager._specialAttackUnlocked;
        _slideUnlocked = _gameManager._slideUnlocked;

        //Set original variables values
        _originalFallingSpeed = _maxFallingSpeed;
        _originalMoveSpeed = _moveSpeed;
        _healPotionsLeft = _maxHealPotions;
        _maxLife += _lifeUpgradeLevel;
        life = _maxLife;
        _maxHealPotions = _healBottlesUpgradeLevel;
        _healPotionsLeft = _maxHealPotions;
        _haelLifeAmount += _healAmountUpgradeLevel;

        //Get Hierarchy Elements
        _cameraController = FindFirstObjectByType<CameraController>();
        _pause = FindFirstObjectByType<PauseScript>();
    }

    private void Start() {
        //Setting UI hability icons (if it is locked or unlocked)
        if(_maxHealPotions > 0) GameObject.Find("HealPotion_Locked").SetActive(false);
        if(_specialAttackUnlocked) GameObject.Find("SpecialAttack_Locked").SetActive(false);
        
        //Setting UI Life size
        Transform healthBar = GameObject.Find("HealthBar").transform;
        RectTransform back = healthBar.Find("Back").GetComponent<RectTransform>();
        RectTransform redLife = healthBar.Find("RedBar").GetComponent<RectTransform>();
        RectTransform yellowLife = healthBar.Find("YellowBar").GetComponent<RectTransform>();
        RectTransform frame = healthBar.Find("Frame").GetComponent<RectTransform>();
        float increseAmount = _lifeSizeIncreasePerPoint * _lifeUpgradeLevel;
        back.sizeDelta = new Vector2(back.sizeDelta[0] + increseAmount, back.sizeDelta[1]);
        redLife.sizeDelta = new Vector2(redLife.sizeDelta[0] + increseAmount, redLife.sizeDelta[1]);
        yellowLife.sizeDelta = new Vector2(yellowLife.sizeDelta[0] + increseAmount, yellowLife.sizeDelta[1]);
        frame.sizeDelta = new Vector2(frame.sizeDelta[0] + increseAmount, frame.sizeDelta[1]);
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

        //Set _isHealing to false if hit animation already ended
        if (_isHealing && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Adventurer_Healing")) {
            _canMove = true;
            _isHealing = false;
        }
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

        if (((_canJump && !_isSliding) || _canWallJump) && !_isGettingHit && !_isUsingSpecialAttack && !_isHealing && !_isDead)
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
        if (Mathf.Abs(_moveDirection.x) > 0 && _slideUnlocked && _canJump && !_isSliding && (_lastSlideTime + _slideCooldown <= Time.time) && !_isAttacking && !_isGettingHit && !_isUsingSpecialAttack && !_isDead)
        {
            _canMove = false;
            _isSliding = true;
            _rb.AddForce(new Vector2(_moveDirection.x, 0) * _slideForce, ForceMode2D.Impulse);
            _roolStartVelocity = Mathf.Abs(_rb.linearVelocityX);
            _animator.SetBool(Constants.ANIM_IS_SLIDING, true);
        }
    }

    private void OnSpecialAttack() {
        if((Time.time >= _lastSpecialAttackTime + _specialAttackCooldown - _specialCooldownUpgradeLevel) && _specialAttackUnlocked && !_isJumping && !_isDead && !_isAttacking && !_isGettingHit && !_isSliding && !_isHealing && !_isUsingSpecialAttack && !_pause.isPaused && _canUseSpecialAttack) {
            _lastSpecialAttackTime = Time.time;
            SFXManager.TocarSFX(2);
            _rb.linearVelocityX = 0;
            _isUsingSpecialAttack = true;
            _animator.SetTrigger(Constants.ANIM_SPECIAL_ATTACK);
            StartCoroutine(SpecialAttackCooldown());
        }
    }

    private void OnHeal() {
        if(_healPotionsLeft > 0 && life != _maxLife && !_isDead && !_isAttacking && !_isGettingHit && !_isSliding && !_isJumping && !_isUsingSpecialAttack && !_pause.isPaused && !_isHealing) {
            //Setting correct values for variables
            _healPotionsLeft--;
            _canMove = false;
            _isHealing = true;
            _rb.linearVelocityX = 0; //Stop moving

            //Setting UI
            print("Amount left: " + _healPotionsLeft/_maxHealPotions);
            _healPotionAnimator.SetFloat("AmountLeft", (float) _healPotionsLeft/_maxHealPotions);
            if(_healPotionsLeft == 0) {
                _healPotionCommandUI.color = new Color32(136,136,136,255);
                _healPotionBackgroundUI.color = new Color32(136,136,136,255);
                _healPotionIconUI.color = new Color32(136,136,136,255);
            }

            _animator.SetTrigger(Constants.ANIM_HEAL);
            life = Mathf.Min(_maxLife, life + _haelLifeAmount);
        }
    }

    void MovePlayer()
    {
        if(_canMove && !_isWallJumping && !_isUsingSpecialAttack && !_isHealing && !_isDead)
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
        life = Mathf.Max(0, life-damage);
        print("My life: " + life);
        
        if(life > 0)
        {
            SFXManager.TocarSFX(1);
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

    public void UnlockSpecialAttack() {
        _specialAttackUnlocked = true;
        _gameManager._specialAttackUnlocked = true;
        GameObject.Find("SpecialAttack_Locked").SetActive(false);
        GameObject.Find("SpecialAttackUnlocked").GetComponent<Animator>().SetTrigger("UnlockHability");
    }

    public void UnlockSlide() {
        _slideUnlocked = true;
        _gameManager._slideUnlocked = true;
        GameObject.Find("SlideUnlocked").GetComponent<Animator>().SetTrigger("UnlockHability");
    }

    private void SpawnBegin() {
        _isDead = true;
    }

    private void SpawnEnd() {
        _isDead = false;
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
        SFXManager.TocarSFX(3);
        FindFirstObjectByType<GameManager>().SaveGame();
        yield return new WaitForSecondsRealtime(1.5f);
        _ascendingAnimator.SetTrigger("Ascend");
        SFXManager.TocarSFX(14);
        SFXManager.TrocarMusica(2, 6);
        yield return new WaitForSecondsRealtime(5.6f);
        SceneManager.LoadScene("Upgrade");
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

    IEnumerator SpecialAttackCooldown() {
        _canUseSpecialAttack = false;

        int time = _specialAttackCooldown - _specialCooldownUpgradeLevel;
        _specialAttackTextUI.text = Convert.ToString(time);
        _specialAttackCommandUI.color = new Color32(136,136,136,255);
        _specialAttackBackgroundUI.color = new Color32(136,136,136,255);
        _specialAttackIconUI.color = new Color32(136,136,136,255);

        while(time != 0) {
            _specialAttackTextUI.enabled = true;
            yield return new WaitForSeconds(1);
            _specialAttackTextUI.text = Convert.ToString(--time);
        }
        
        _specialAttackCommandUI.color = new Color32(226,201,165,255);
        _specialAttackBackgroundUI.color = new Color32(255,219,187,255);
        _specialAttackIconUI.color = new Color32(255,255,255,255);
        _specialAttackTextUI.enabled = false;
        _canUseSpecialAttack = true;
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