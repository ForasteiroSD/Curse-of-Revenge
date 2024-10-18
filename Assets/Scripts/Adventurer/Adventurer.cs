using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Adventurer : MonoBehaviour {
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    [SerializeField] private float _moveSpeed = 5f;
    private Animator _animator;
    [SerializeField] private float _analogDeadZone = .3f;

    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update() {
        MovePlayer();
    }

    void OnMove(InputValue inputValue) {
        Vector2 dir = inputValue.Get<Vector2>();

        if (dir.x > _analogDeadZone) _moveDirection = new Vector2(1, dir.y);
        else if (dir.x < -_analogDeadZone) _moveDirection = new Vector2(-1, dir.y);
        else _moveDirection = new Vector2(0, dir.y);
    }

    void MovePlayer() {
        Vector3 scale = transform.localScale;
        bool isRunning = Mathf.Abs(_moveDirection.x) > Mathf.Epsilon;

        _rb.linearVelocityX = _moveDirection.x * _moveSpeed;
        if (isRunning) {
            transform.localScale = new Vector3(Mathf.Sign(_moveDirection.x) * Mathf.Abs(scale.x), scale.y, scale.z);
            _animator.SetBool(Constants.ANIM_IS_RUNNING, true);
        } else {
            _animator.SetBool(Constants.ANIM_IS_RUNNING, false);
        }
    }
}