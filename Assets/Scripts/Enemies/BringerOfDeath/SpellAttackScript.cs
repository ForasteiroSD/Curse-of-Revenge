using UnityEngine;

public class SpellAttackScript : MonoBehaviour
{
    BringerScript _script;

    void Awake()
    {
        _script = FindFirstObjectByType<BringerScript>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameObject.activeInHierarchy) _script.GiveDamage();
        }
    }

    //called by spell animation
    void StopSpell()
    {
        if(gameObject.activeInHierarchy) _script.StopSpell();
    }

}