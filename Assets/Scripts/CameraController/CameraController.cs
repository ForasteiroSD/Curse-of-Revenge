using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utils;

public class CameraController : MonoBehaviour
{
    //Varibles
    private bool _isAttacking = false;
    private bool _canWallJump = false;
    private float _lastMovedDirection;
    private float _yOffsetIncreaser;
    [SerializeField] private float _minFallingSpeedToOffset;
    [SerializeField] private float _originalYOffsetIncreaser = 0.02f;
    [SerializeField] private float _maxOffsetY = 2f;
    [SerializeField] private float _offsetX = 1.5f;
    [SerializeField] private float _fallOffsetDivisor = 5;

    //Component references
    private CinemachinePositionComposer _composer;
    private Adventurer _adventurer;
    private GameObject _player;
    private Rigidbody2D _rb;
    
    void Awake() {
        //Getting element references
        _player = GameObject.Find(Constants.HIERARCHY_PLAYER);

        //Getting component references
        _composer = GameObject.Find(Constants.HIERARCHY_CINEMACHINE_CAMERA).GetComponent<CinemachinePositionComposer>();
        _adventurer = GameObject.Find(Constants.HIERARCHY_PLAYER).GetComponent<Adventurer>();
        _rb = GameObject.Find(Constants.HIERARCHY_PLAYER).GetComponent<Rigidbody2D>();
        
        _minFallingSpeedToOffset = _adventurer._maxFallingSpeed;
        _yOffsetIncreaser = _originalYOffsetIncreaser;
    }

    void FixedUpdate() {
        if(_isAttacking && !_adventurer._isAttacking) {
            _isAttacking = false;
            Moved(_lastMovedDirection);
        }

        if(_canWallJump && !_adventurer._canWallJump) {
            _canWallJump = false;
            Moved(_lastMovedDirection);
        }

        if(_rb.linearVelocityY <= -_minFallingSpeedToOffset) {
            if(_composer.TargetOffset.y > -_maxOffsetY){
                // print("A");
                _composer.TargetOffset.y -= _yOffsetIncreaser;
                _yOffsetIncreaser += _originalYOffsetIncreaser;
            }
            else {
                // print("B");
                _yOffsetIncreaser = _originalYOffsetIncreaser;
            }
        }
        else {
            // if(_composer.TargetOffset.y < 0) {
            //     // print("C");
            //     _composer.TargetOffset.y += _yOffsetIncreaser;
            //     _yOffsetIncreaser += _originalYOffsetIncreaser;
            // }
            // else {
            //     // print("D");
            //     _yOffsetIncreaser = _originalYOffsetIncreaser;
            // }
            _composer.TargetOffset.y = 0;
        }
    }

    public void Moved(float newDirecion) {
        if(_adventurer._isAttacking) {
            _isAttacking = true;
            _lastMovedDirection = newDirecion;
            return;
        }

        if(_adventurer._canWallJump) {
            _canWallJump = true;
            _lastMovedDirection = newDirecion;
            return;
        }

        if(newDirecion == 0) _composer.TargetOffset.x = _offsetX *  Mathf.Sign(_player.transform.localScale.x);
        else _composer.TargetOffset.x = _composer.Damping.x * Mathf.Sign(newDirecion);
    }
}
