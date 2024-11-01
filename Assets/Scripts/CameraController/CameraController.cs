using Unity.Cinemachine;
using UnityEngine;
using Utils;

public class CameraController : MonoBehaviour
{
    //Varibles
    // private float _targetOffsetX;
    // private bool _hasToMoveX = false;
    [SerializeField] private float _offsetX = 1.5f;
    // [SerializeField] private float _offsetXCorrection = .05f;

    //Component references
    private CinemachinePositionComposer _composer;
    private GameObject _player;
    
    void Awake() {
        //Getting element references
        _player = GameObject.Find(Constants.HIERARCHY_PLAYER);

        //Getting component references
        _composer = GameObject.Find(Constants.HIERARCHY_CINEMACHINE_CAMERA).GetComponent<CinemachinePositionComposer>();

        //Setting default values
        // _targetOffsetX = _composer.FollowOffset.x;
        // _movingffsetX = _stopedOffsetX + _dampingX;
    }

    void FixedUpdate() {
        // if (_hasToMoveX && _composer != null) {
        //     if(_composer.TargetOffset.x > _targetOffsetX) _composer.TargetOffset.x -= (_composer.TargetOffset.x - _targetOffsetX) * _offsetXCorrection;
        //     else if(_composer.TargetOffset.x < _targetOffsetX) _composer.TargetOffset.x += (_targetOffsetX - _composer.TargetOffset.x) * _offsetXCorrection;
        //     else _hasToMoveX = false;
        // }
    }

    public void Moved(float previousDirection, float newDirecion) {
        // if(newDirecion == 0) {
        //     _targetOffsetX = _stopedOffsetX * Mathf.Sign(_player.transform.localScale.x);
        // } else {
        //     _targetOffsetX = _stopedOffsetX * Mathf.Sign(_player.transform.localScale.x);
        // }

        if(newDirecion == 0) _composer.TargetOffset.x = _offsetX *  Mathf.Sign(previousDirection);
        else _composer.TargetOffset.x = (_offsetX + _composer.Damping.x) * Mathf.Sign(newDirecion);

        // _hasToMoveX = true;
    }
}
