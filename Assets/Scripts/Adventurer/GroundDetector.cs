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
        //Touched ground
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            _animator.SetBool(Constants.ANIM_IS_FALLING, false);
            _adventurer._canJump = true;

            //If player pressed the jump button a few time earlier, still consider the jump
            if (Time.time <= _adventurer._lastJumpTime + _adventurer._preJumpTimeLimit) _adventurer.OnJump();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //While on ground, can jump again
        if (collision.CompareTag(Constants.TAG_GROUND)) _adventurer._canJump = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //When leavin the ground
        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            _animator.SetBool(Constants.ANIM_IS_FALLING, true);
            _animator.SetTrigger(Constants.ANIM_FALL);
            if (_player.activeInHierarchy) StartCoroutine(CancelCanJump());
        }
    }

    IEnumerator CancelCanJump()
    {
        yield return new WaitForSeconds(_adventurer._acceptJumpTime);
        _adventurer._canJump = false;
    }
}
