using UnityEngine;

public abstract class BossScript : MonoBehaviour
{

    AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    protected void PlayAudio(int index)
    {
        audioManager.TocarSFX(index);
    }

    public abstract void GetHit(float damage);

}