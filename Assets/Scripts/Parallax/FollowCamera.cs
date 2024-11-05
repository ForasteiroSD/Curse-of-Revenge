using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    //[SerializeField] private bool _followX = true;
    //[SerializeField] private bool _followY = false;


    void Update()
    {
        transform.position = new Vector3(_camera.transform.position.x, transform.position.y, transform.position.z);
    }
}