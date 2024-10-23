using UnityEngine;

public class FlyHeightScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<FlyingEyeScript>().GoDown();
    }
}
