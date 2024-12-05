using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckAnimationEnd : MonoBehaviour
{
    void AnimationFinished() {
        SceneManager.LoadScene("MainMenu");
    }
}