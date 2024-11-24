using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //Player stats
    public int _lifeUpgradeLevel { get; private set; } = 0;
    public int _healBottlesUpgradeLevel { get; private set; } = 0;
    public int _healAmountUpgradeLevel { get; private set; } = 0;
    public int _DamageUpgradeLevel { get; private set; } = 0;
    public int _specialCooldownUpgradeLevel { get; private set; } = 0;
    public int[] _lifePrices { get; private set; } = {25, 50, 75, 100, 150, 200, 250, 300, 350, 400, 500, 600, 700};
    public int[] _healBottlesPrices { get; private set; } = {400, 650, 900};
    public int[] _healAmountPrices { get; private set; } = {100, 150, 200, 250, 300, 350, 400, 500, 600, 700};
    public int[] _DamagePrices { get; private set; } = {100, 200, 300, 400, 500, 600};
    public int[] _specialCooldownPrices { get; private set; } = {50, 100, 250, 450, 700};

    //General
    public int _revengePointsAmount { get; private set; } = 0;

    //Texts
    private TextMeshProUGUI level;
    private TextMeshProUGUI price;
    private TextMeshProUGUI revengePoints;

    private void Awake() {
        LoadGame();
    }

    public void SaveGame() {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame() {
        GameData data = SaveSystem.LoadGame();

        if(data == null) return;

        _lifeUpgradeLevel = data._lifeUpgradeLevel;
        _healBottlesUpgradeLevel = data._healBottlesUpgradeLevel;
        _healAmountUpgradeLevel = data._healAmountUpgradeLevel;
        _DamageUpgradeLevel = data._DamageUpgradeLevel;
        _specialCooldownUpgradeLevel = data._specialCooldownUpgradeLevel;
        _revengePointsAmount = data._revengePointsAmount;
    }

    private void FindTextElements(string name) {
        GameObject life = GameObject.Find(name);
        level = life.transform.Find("Image").Find("Level").GetComponent<TextMeshProUGUI>();
        price = life.transform.Find("LeftSide").Find("RevengePoints").Find("Cost").GetComponent<TextMeshProUGUI>();
        revengePoints = GameObject.Find("RevengeQuantity").GetComponent<TextMeshProUGUI>();
    }

    // Update Functions
    public void UpdateLife() {
        FindTextElements("Life");

        if(_lifeUpgradeLevel < _lifePrices.Length) {
            _lifeUpgradeLevel++;

            if(_lifeUpgradeLevel < _lifePrices.Length) {
                level.text = (_lifeUpgradeLevel + 1).ToString();
                price.text = _lifePrices[_lifeUpgradeLevel].ToString("000");
            } else {
                level.text = "MAX";
                level.fontSize = 32;
            }
        }
    }
    
    // Update Functions
    public void UpdateHealBottle(TextMeshProUGUI level, TextMeshProUGUI price, TextMeshProUGUI revengePoints) {
        _healBottlesUpgradeLevel++;
        level.text = (_healBottlesUpgradeLevel + 1).ToString();
        price.text = _lifePrices[_lifeUpgradeLevel].ToString();
    }
    // Update Functions
    public void UpdateHealAmount(TextMeshProUGUI level, TextMeshProUGUI price, TextMeshProUGUI revengePoints) {
        _healAmountUpgradeLevel++;
        level.text = (_healAmountUpgradeLevel + 1).ToString();
        price.text = _lifePrices[_lifeUpgradeLevel].ToString();
    }
    // Update Functions
    public void UpdateDamage(TextMeshProUGUI level, TextMeshProUGUI price, TextMeshProUGUI revengePoints) {
        _DamageUpgradeLevel++;
        level.text = (_DamageUpgradeLevel + 1).ToString();
        price.text = _lifePrices[_lifeUpgradeLevel].ToString();
    }
    // Update Functions
    public void UpdateCooldown(TextMeshProUGUI level, TextMeshProUGUI price, TextMeshProUGUI revengePoints) {
        _specialCooldownUpgradeLevel++;
        level.text = (_specialCooldownUpgradeLevel + 1).ToString();
        price.text = _lifePrices[_lifeUpgradeLevel].ToString();
    }
    // Update Functions
    public void UpdateRevengePoints(TextMeshProUGUI level, TextMeshProUGUI revengePoints) {
        _revengePointsAmount++;
        level.text = (_revengePointsAmount + 1).ToString();
    }
}