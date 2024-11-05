using UnityEngine;

public class MushroomFeetScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy) gameObject.GetComponentInParent<MushroomScript>().GetOnBorder();
    }
}
