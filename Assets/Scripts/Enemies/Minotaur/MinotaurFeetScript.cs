using UnityEngine;

public class MinotaurFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<MinotaurScript>().GetOnBorder();
    }
}
