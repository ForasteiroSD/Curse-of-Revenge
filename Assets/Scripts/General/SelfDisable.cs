using UnityEngine;

public class SelfDisable : MonoBehaviour {
    public void Disable() {
        this.gameObject.SetActive(false);
    }
}