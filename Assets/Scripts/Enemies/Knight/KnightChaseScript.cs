using UnityEngine;

public class KnightChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeInHierarchy) GetComponentInParent<KnightScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<KnightScript>().PlayerInRange();
    }
}