using UnityEngine;

public class NBAttackScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<NBScript>().target = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<NBScript>().target = null;
    }
}
