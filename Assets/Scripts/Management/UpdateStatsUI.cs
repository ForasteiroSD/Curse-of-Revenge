using TMPro;
using UnityEngine;

public class UpdateStatsUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _lifePriceText;
    [SerializeField] private TextMeshProUGUI _healBottlesText;
    [SerializeField] private TextMeshProUGUI _healBottlesPriceText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _damagePriceText;
    [SerializeField] private TextMeshProUGUI _healAmountText;
    [SerializeField] private TextMeshProUGUI _healAmountPriceText;
    [SerializeField] private TextMeshProUGUI _specialDamageText;
    [SerializeField] private TextMeshProUGUI _specialDamagePriceText;
    [SerializeField] private TextMeshProUGUI _specialCooldownText;
    [SerializeField] private TextMeshProUGUI _specialCooldownPriceText;
    [SerializeField] private TextMeshProUGUI _revengePointsText;
    private GameManager _gameMenager;

    private void Start() {
        //Getting Scripts
        _gameMenager = FindFirstObjectByType<GameManager>();

        // Updating levels
        _lifeText.text = (_gameMenager._lifeUpgradeLevel + 1).ToString();
        _healBottlesText.text = _gameMenager._healBottlesUpgradeLevel.ToString();
        _healAmountText.text = (_gameMenager._healAmountUpgradeLevel + 1).ToString();
        _damageText.text = (_gameMenager._damageUpgradeLevel + 1).ToString();
        _specialDamageText.text = (_gameMenager._specialDamageUpgradeLevel + 1).ToString();
        _specialCooldownText.text = (_gameMenager._specialCooldownUpgradeLevel + 1).ToString();
        _revengePointsText.text = _gameMenager._revengePointsAmount.ToString("0000");

        // Updating prices
        if(_gameMenager._lifeUpgradeLevel < _gameMenager._lifePrices.Length) _lifePriceText.text = _gameMenager._lifePrices[_gameMenager._lifeUpgradeLevel].ToString("000");
        else SetMaxLevel(_lifeText, _lifePriceText);
        if(_gameMenager._healBottlesUpgradeLevel < _gameMenager._healBottlesPrices.Length) _healBottlesPriceText.text = _gameMenager._healBottlesPrices[_gameMenager._healBottlesUpgradeLevel].ToString("000");
        else SetMaxLevel(_healBottlesText, _healBottlesPriceText);
        if(_gameMenager._damageUpgradeLevel < _gameMenager._damagePrices.Length) _damagePriceText.text = _gameMenager._damagePrices[_gameMenager._damageUpgradeLevel].ToString("000");
        else SetMaxLevel(_damageText, _damagePriceText);
        if(_gameMenager._healAmountUpgradeLevel < _gameMenager._healAmountPrices.Length) _healAmountPriceText.text = _gameMenager._healAmountPrices[_gameMenager._healAmountUpgradeLevel].ToString("000");
        else SetMaxLevel(_healAmountText, _healAmountPriceText);
        if(_gameMenager._specialDamageUpgradeLevel < _gameMenager._specialDamagePrices.Length) _specialDamagePriceText.text = _gameMenager._specialDamagePrices[_gameMenager._specialDamageUpgradeLevel].ToString("000");
        else SetMaxLevel(_specialDamageText, _specialDamagePriceText);
        if(_gameMenager._specialCooldownUpgradeLevel < _gameMenager._specialCooldownPrices.Length) _specialCooldownPriceText.text = _gameMenager._specialCooldownPrices[_gameMenager._specialCooldownUpgradeLevel].ToString("000");
        else SetMaxLevel(_specialCooldownText, _specialCooldownPriceText);
    }

    private void SetMaxLevel(TextMeshProUGUI level, TextMeshProUGUI price) {
        level.text = "MAX";
        level.fontSize = 32;
        price.text = "---";
        price.transform.parent.parent.Find("Upgrade").gameObject.SetActive(false);
    }
}