using UnityEngine;

public class FlyingAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<FlyingEyeScript>().GiveDamage();
        }
    }
}
