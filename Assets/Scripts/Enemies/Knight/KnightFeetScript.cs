using UnityEngine;

public class KnightFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<KnightScript>().GetOnBorder();
    }
}