using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxController : MonoBehaviour {
    private float _lenght;
    private float _startPos;
    private Transform _cam;
    [SerializeField] private float _vellocity;

    private void Start() {
        _startPos = transform.position.x;
        _lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        _cam = Camera.main.transform;
        print(_cam);
    }

    private void FixedUpdate() {
        float newPos = _cam.transform.position.x * (1 - _vellocity);
        float distance = _cam.transform.position.x * _vellocity;

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);

        if(newPos > _startPos + _lenght) {
            _startPos += _lenght;
        }
        else if(newPos < _startPos - _lenght) {
            _startPos -= _lenght;
        }
    }
}