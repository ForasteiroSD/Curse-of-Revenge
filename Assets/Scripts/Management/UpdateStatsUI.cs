using TMPro;
using UnityEngine;

public class UpdateStatsUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _healBottlesText;
    [SerializeField] private TextMeshProUGUI _healAmountText;
    [SerializeField] private TextMeshProUGUI _DamageText;
    [SerializeField] private TextMeshProUGUI _specialCooldownText;
    [SerializeField] private TextMeshProUGUI _revengePointsText;
    [SerializeField] private GameManager _gameMenager;

    private void Start() {
        _lifeText.text = (_gameMenager._lifeUpgradeLevel + 1).ToString();
        _healBottlesText.text = (_gameMenager._healBottlesUpgradeLevel + 1).ToString();
        _healAmountText.text = (_gameMenager._healAmountUpgradeLevel + 1).ToString();
        _DamageText.text = (_gameMenager._DamageUpgradeLevel + 1).ToString();
        _specialCooldownText.text = (_gameMenager._specialCooldownUpgradeLevel + 1).ToString();
        _revengePointsText.text = _gameMenager._revengePointsAmount.ToString("0000");
    }
}