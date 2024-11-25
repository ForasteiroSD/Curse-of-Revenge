using TMPro;
using UnityEngine;

public class UpdateRevengePointsUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _revengePointsUIText;

    private void Start() {
        _revengePointsUIText.text = FindFirstObjectByType<GameManager>()._revengePointsAmount.ToString("0000");
    }
}