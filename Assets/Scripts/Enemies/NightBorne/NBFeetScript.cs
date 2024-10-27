using UnityEngine;

public class NBFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<NBScript>().GetOnBorder();
    }
}
