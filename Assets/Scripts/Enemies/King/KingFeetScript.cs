using UnityEngine;

public class KingFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<KingScript>().GetOnBorder();
    }
}