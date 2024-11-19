using UnityEngine;

public class GoblinChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeInHierarchy) GetComponentInParent<GoblinScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<GoblinScript>().PlayerInRange();
    }
}