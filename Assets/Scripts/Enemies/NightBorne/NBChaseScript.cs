using UnityEngine;
using Utils;

public class NBChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<NBScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<NBScript>().PlayerInRange();
    }
}
