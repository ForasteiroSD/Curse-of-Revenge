using System.Collections;
using UnityEngine;

public class ChestRoomDoorController : MonoBehaviour
{
    [SerializeField] GameObject _door;
    [SerializeField] GameObject _enemies;
    BoxCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        _enemies.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _enemies.SetActive(true);
        _door.GetComponent<Animator>().SetTrigger("Close");
        Destroy(_collider);
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        while (_enemies.GetComponentInChildren<EnemiesScript>() != null)
        {
            yield return new WaitForSecondsRealtime(1);
        }

        _door.GetComponent<Animator>().SetTrigger("Open");

        yield return new WaitForSecondsRealtime(0.5f);

        Destroy(_door);
        Destroy(_enemies);
        Destroy(gameObject);
    }
}
