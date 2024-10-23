using UnityEngine;

public class FlyingEyeChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<FlyingEyeScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<FlyingEyeScript>().PlayerInRange();
    }
}
