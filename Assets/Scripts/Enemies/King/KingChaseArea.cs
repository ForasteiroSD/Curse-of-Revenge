using UnityEngine;

public class KingChaseArea : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeInHierarchy) GetComponentInParent<KingScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<KingScript>().PlayerInRange();
    }
}