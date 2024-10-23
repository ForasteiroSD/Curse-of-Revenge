using UnityEngine;
using Utils;

public class FeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<SkeletonScript>().GetOnBorder();
    }
}
