using UnityEngine;

public class TransitionManager : MonoBehaviour {
    private GameManager _gameManager;

    private void Awake() {
        _gameManager = FindFirstObjectByType<GameManager>();

        if(_gameManager._level == 1 && _gameManager._alreadyDied) {
            FindFirstObjectByType<Adventurer>().gameObject.GetComponent<Animator>().SetTrigger("Spawn");
            GameObject ascendingBackground = GameObject.Find("AscendBackGround");
            if(ascendingBackground != null) ascendingBackground.GetComponent<Animator>().SetTrigger("Respawn");
        }
        else {
            GameObject transitionBackground = GameObject.Find("Transition");
            transitionBackground.GetComponent<Animator>().SetTrigger("FadeIn");
        }
    }
}
