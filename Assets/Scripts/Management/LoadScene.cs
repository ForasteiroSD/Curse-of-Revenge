using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
    private void Start() {
        SceneManager.LoadScene("Level " + FindFirstObjectByType<GameManager>()._level);
    }
}
