using System.Collections;
using UnityEngine;
using Utils;

public class GroundDetector : MonoBehaviour
{
    //GameObjects
    private GameObject _player;

    //Components
    private Animator _animator;

    //Scripts
    private Adventurer _adventurer;
    private Collider2D _collider;
    private Rigidbody2D _playerRb;

    private void Awake()
    {
        //Get Components
        _animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        _adventurer = gameObject.transform.parent.gameObject.GetComponent<Adventurer>();
        _collider = GetComponent<Collider2D>();
        _playerRb = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();

        //Find objects
        _player = GameObject.Find(Constants.HIERARCHY_PLAYER);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Touched ground
        if (collision.CompareTag(Constants.TAG_GROUND) || collision.CompareTag(Constants.TAG_PLATFORM))
        {
            _adventurer._isJumping = false;
            _adventurer._canJump = true;
            _adventurer._canMove = true;

            //If player pressed the jump button a few time earlier, still consider the jump
            if (Time.time <= _adventurer._lastJumpTime + _adventurer._preJumpTimeLimit)
            {
                _adventurer._considerPreJump = false;
                _adventurer.OnJump();
            }

            //If player pressed the slide button a few time earlier, still consider the slide
            if (Time.time <= _adventurer._lastSlideAttemptTime + _adventurer._preSlideTimeLimit) _adventurer.OnSlide();

            //If player touch the ground when is wall sliding, cancel canWallJump and turn him back do the right position
            if (_adventurer._canWallJump)
            {
                GameObject playerGO = transform.parent.gameObject;
                Vector3 scale = playerGO.transform.localScale;

                playerGO.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                _animator.SetBool(Constants.ANIM_IS_WALL_SLIDING, false);
                _adventurer._canWallJump = false;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //While on ground, can jump again
        if (collision.CompareTag(Constants.TAG_GROUND) || collision.CompareTag(Constants.TAG_PLATFORM)) {
               _adventurer._canJump = true;

               if(Mathf.Abs(_playerRb.linearVelocityY) <= 0.0001) _animator.SetBool(Constants.ANIM_IS_FALLING, false);
            //    _animator.SetBool(Constants.ANIM_IS_FALLING, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //When leaving the ground
        if ((collision.CompareTag(Constants.TAG_GROUND) || collision.CompareTag(Constants.TAG_PLATFORM)) && _player.activeInHierarchy && !_collider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        // if (collision.CompareTag(Constants.TAG_GROUND) && _player.activeInHierarchy)
        {
            if (!_animator.GetBool(Constants.ANIM_IS_FALLING) && !_adventurer._isDead)
            {
                _animator.SetTrigger(Constants.ANIM_FALL);
                _animator.SetBool(Constants.ANIM_IS_FALLING, true);
            }
            StartCoroutine(CancelCanJump());
        }
    }

    IEnumerator CancelCanJump()
    {
        yield return new WaitForSeconds(_adventurer._acceptJumpTime);
        _adventurer._canJump = false;
    }
}