using System.Collections;
using UnityEngine;

public class ChestRoomDoorController : MonoBehaviour
{
    [SerializeField] GameObject _door;
    [SerializeField] GameObject _enemies;
    [SerializeField] GameObject _chest;
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
        Destroy(_collider);
        if(Random.Range(0, 2) == 0)
        {
            _enemies.SetActive(true);
            _door.GetComponent<Animator>().SetTrigger("Close");
            StartCoroutine(OpenDoor());
        }
        else
        {
            _chest.GetComponent<CapsuleCollider2D>().enabled = true;
        }
    }

    IEnumerator OpenDoor()
    {

        yield return new WaitForSeconds(1.5f);
        
        _enemies.GetComponent<Animator>().enabled = false;

        int count;
        while (true)
        {
            count = 0;
            EnemiesScript[] enemies = _enemies.GetComponentsInChildren<EnemiesScript>();
            foreach (EnemiesScript enemy in enemies)
            {
                if(enemy._death) count++;
            }
            if(count == enemies.Length) break;
            yield return new WaitForSeconds(1);
        }

        _door.GetComponent<Animator>().SetTrigger("Open");
        _chest.GetComponent<CapsuleCollider2D>().enabled = true;

        yield return new WaitForSeconds(4f);

        Destroy(_door);
        Destroy(_enemies);
        Destroy(gameObject);
    }
}
