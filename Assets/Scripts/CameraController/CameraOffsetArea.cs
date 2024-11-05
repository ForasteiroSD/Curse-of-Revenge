using System.Collections;
using UnityEngine;

public class CameraOffsetArea : MonoBehaviour
{
    private bool _hasToMove = false;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;
    [SerializeField] private float _MovingDamping;
    [SerializeField] private float _delayToMove = 1;

    private CameraController _cameraController;

    private void Awake() {
        _cameraController = FindFirstObjectByType<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        _hasToMove = true;
        StartCoroutine(MoveCamera());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _cameraController.ResetMovedCamera();
    }

    IEnumerator MoveCamera() {
        yield return new WaitForSeconds(_delayToMove);
        if(_hasToMove) {
            _cameraController.MoveCamera(_xOffset, _yOffset, _MovingDamping);
            _hasToMove = false;
        }
    }
}
