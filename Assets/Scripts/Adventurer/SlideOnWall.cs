using UnityEngine;
using Utils;

public class SlideOnWall : MonoBehaviour
{
    //Components
    private Rigidbody2D _playerRb;
    private Animator _animator;

    //Scripts
    private Adventurer _adventurer;

    //Wall slide
    private float _originalFallingSpeed;

    private void Awake()
    {
        //Get Components
        _animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        _adventurer = gameObject.transform.parent.gameObject.GetComponent<Adventurer>();
        _playerRb = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        _originalFallingSpeed = _adventurer._maxFallingSpeed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.TAG_GROUND) && _playerRb.linearVelocityY < 0)
        {
            _animator.SetBool(Constants.ANIM_IS_WALL_SLIDING, true);
            _adventurer._maxFallingSpeed = _adventurer._wallMaxFallingSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            _animator.SetBool(Constants.ANIM_IS_WALL_SLIDING, false);
            _adventurer._maxFallingSpeed = _originalFallingSpeed;
        }
    }
}
