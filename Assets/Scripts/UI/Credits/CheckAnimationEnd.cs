using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckAnimationEnd : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimationFinished = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!hasAnimationFinished && animator.GetCurrentAnimatorStateInfo(0).IsName("Credits"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                hasAnimationFinished = true;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
