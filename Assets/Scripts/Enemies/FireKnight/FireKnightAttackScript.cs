using UnityEngine;

public class FireKnightAttackScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy) gameObject.GetComponentInParent<FireKnightScript>().GiveDamage();
        }
    }
}
