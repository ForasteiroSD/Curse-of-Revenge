using UnityEngine;
using Utils;

public class Adventurer_Attack_Hit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.TAG_ENEMY))
        {
            collision.gameObject.GetComponent<SkeletonScript>().GetHit(1);
        }
    }
}