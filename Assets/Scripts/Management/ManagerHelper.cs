using UnityEngine;

public class ManagerHelper : MonoBehaviour {
    private GameManager _gameManager;
    
    private void Start() {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    public void UpdateLife() {
        _gameManager.UpdateLife();
    }
    
    public void UpdateHealBottle() {
        _gameManager.UpdateHealBottle();
    }
    
    public void UpdateHealAmount() {
        _gameManager.UpdateHealAmount();
    }
    
    public void UpdateDamage() {
        _gameManager.UpdateDamage();
    }

    public void UpdateSpecialDamage() {
        _gameManager.UpdateSpecialDamage();
    }
    
    public void UpdateSpecialCooldown() {
        _gameManager.UpdateSpecialCooldown();
    }

    public void FinishUpdate() {
        _gameManager.FinishUpdate();
    }
}
