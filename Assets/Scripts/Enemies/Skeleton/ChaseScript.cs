using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Utils;

public class ChaseScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeInHierarchy) GetComponentInParent<SkeletonScript>().PlayerLeaveRange();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) GetComponentInParent<SkeletonScript>().PlayerInRange();
    }
}
