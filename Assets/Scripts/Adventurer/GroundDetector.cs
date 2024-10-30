using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Utils;
using System.Collections;

public class GroundDetector : MonoBehaviour
{
    //GameObjects
    private GameObject _player;

    //Components
    private Animator _animator;

    //Scripts
    private Adventurer _adventurer;
    
    // Ground detection delay
    private float fallDelay = 0.2f; // Defina um valor de delay conforme necessário
    private Coroutine fallCoroutine;

    private void Awake()
    {
        //Get Components
        _animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        _adventurer = gameObject.transform.parent.gameObject.GetComponent<Adventurer>();

        //Find objects
        _player = GameObject.Find(Constants.HIERARCHY_PLAYER);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Touched ground
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            if (fallCoroutine != null)
            {
                StopCoroutine(fallCoroutine); // Para o delay de queda se estiver em execução
            }

            _adventurer._canJump = true;
            _adventurer._canMove = true;
            _animator.SetBool(Constants.ANIM_IS_FALLING, false);

            // If player pressed the jump button a few time earlier, still consider the jump
            if (Time.time <= _adventurer._lastJumpTime + _adventurer._preJumpTimeLimit) 
                _adventurer.OnJump();

            // If player pressed the slide button a few time earlier, still consider the slide
            if (Time.time <= _adventurer._lastSlideAttemptTime + _adventurer._preSlideTimeLimit) 
                _adventurer.OnSlide();

            // If player touch the ground when is wall sliding, cancel canWallJump and turn him back to the right position
            if (_adventurer._canWallJump)
            {
                GameObject playerGO = transform.parent.gameObject;
                Vector3 scale = playerGO.transform.localScale;
                playerGO.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                _adventurer._canWallJump = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // While on ground, can jump again
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            _adventurer._canJump = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When leaving the ground, start the fall delay coroutine
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            if (fallCoroutine != null)
            {
                StopCoroutine(fallCoroutine);
            }
            fallCoroutine = StartCoroutine(DelayFallDetection());
        }
    }

    private IEnumerator DelayFallDetection()
    {
        yield return new WaitForSeconds(fallDelay);

        // Confirm if the player is still not touching the ground after the delay
        Collider2D groundCheck = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask(Constants.TAG_GROUND));
        if (groundCheck == null)
        {
            _animator.SetTrigger(Constants.ANIM_FALL);
            _animator.SetBool(Constants.ANIM_IS_FALLING, true);
            if (_player.activeInHierarchy) StartCoroutine(CancelCanJump());
        }
    }

    private IEnumerator CancelCanJump()
    {
        yield return new WaitForSeconds(_adventurer._acceptJumpTime);
        _adventurer._canJump = false;
    }
}