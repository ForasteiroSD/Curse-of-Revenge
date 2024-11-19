using UnityEngine;

public class WizardFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<WizardScript>().GetOnBorder();
    }
}