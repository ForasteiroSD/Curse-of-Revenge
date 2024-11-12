using UnityEngine;

public class MinotaurAttackScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy) gameObject.GetComponentInParent<MinotaurScript>().GiveDamage();
        }
    }
}
