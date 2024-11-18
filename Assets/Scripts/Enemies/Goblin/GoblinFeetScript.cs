using UnityEngine;

public class GoblinFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<GoblinScript>().GetOnBorder();
    }
}