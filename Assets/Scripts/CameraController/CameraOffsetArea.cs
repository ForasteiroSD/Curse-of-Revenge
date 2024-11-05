using System.Collections;
using UnityEngine;
using Utils;

public class CameraOffsetArea : MonoBehaviour
{
    private Animator _adventurerAnimator;
    private bool _isCameraMoved = false;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;
    [SerializeField] private float _MovingDamping;
    [SerializeField] private float _delayToMove = 1;

    private CameraController _cameraController;

    private void Awake() {
        _cameraController = FindFirstObjectByType<CameraController>();
        _adventurerAnimator = GameObject.Find(Constants.HIERARCHY_PLAYER).GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(AnimatorIsPlaying("Adventurer_Idle") && !_isCameraMoved) {
            _isCameraMoved = true;
            StartCoroutine(MoveCamera());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _cameraController.ResetMovedCamera();
        _isCameraMoved = false;
    }

    IEnumerator MoveCamera() {
        yield return new WaitForSeconds(_delayToMove);
        if(_isCameraMoved) _cameraController.MoveCamera(_xOffset, _yOffset, _MovingDamping);
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return _adventurerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
