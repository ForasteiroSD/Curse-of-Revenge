using UnityEngine;

public class WizardChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeInHierarchy) GetComponentInParent<WizardScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<WizardScript>().PlayerInRange();
    }
}