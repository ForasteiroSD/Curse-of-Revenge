using UnityEngine;

public class BringerAttackScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy) gameObject.GetComponentInParent<BringerScript>().GiveDamage();
        }
    }
}
