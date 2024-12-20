using UnityEngine;
using Utils;

public class SlideOnWall : MonoBehaviour
{
    //Components
    private Rigidbody2D _playerRb;
    private Animator _animator;

    //Scripts
    private Adventurer _adventurer;
    [SerializeField] private float teste;

    private void Awake()
    {
        //Get Components
        _animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        _adventurer = gameObject.transform.parent.gameObject.GetComponent<Adventurer>();
        _playerRb = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If is on wall, player can't move
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            //if (Time.time <= _adventurer._lastJumpTime + _adventurer._preJumpTimeLimit)
            //{
            //    _adventurer._canWallJump = true;
            //    _adventurer._canMove = false;
            //    _adventurer.OnJump();
            //}
            //else
            //{
                
                _adventurer._canWallJump = true;
                _adventurer._canMove = false;
            //}
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //While falling on wall, slide on it
        if (collision.CompareTag(Constants.TAG_GROUND) && _playerRb.linearVelocityY < teste) {
            _adventurer._maxFallingSpeed = _adventurer._wallMaxFallingSpeed;
            _animator.SetBool(Constants.ANIM_IS_WALL_SLIDING, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //After leave the wall, set everything back to normal
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            _animator.SetBool(Constants.ANIM_IS_WALL_SLIDING, false);
            _adventurer._maxFallingSpeed = _adventurer._originalFallingSpeed;
            _adventurer._canWallJump = false;
            _adventurer._canMove = true;
        }
    }
}
