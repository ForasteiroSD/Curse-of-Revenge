using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    //Player stats
    public int _lifeUpgradeLevel { get; private set; } = 0;
    public int _healBottlesUpgradeLevel { get; private set; } = 0;
    public int _healAmountUpgradeLevel { get; private set; } = 0;
    public int _damageUpgradeLevel { get; private set; } = 0;
    public int _specialDamageUpgradeLevel { get; private set; } = 0;
    public int _specialCooldownUpgradeLevel { get; private set; } = 0;
    public int[] _lifePrices { get; private set; } = {25, 50, 75, 100, 150, 200, 250, 300, 350, 450, 500, 550, 600, 700, 800, 900, 999};
    public int[] _healBottlesPrices { get; private set; } = {400, 700, 999};
    public int[] _healAmountPrices { get; private set; } = {100, 150, 200, 250, 300, 350, 400, 500, 600, 700};
    public int[] _damagePrices { get; private set; } = {100, 200, 300, 400, 500, 600, 700, 800, 999};
    public int[] _specialDamagePrices { get; private set; } = {100, 200, 300, 400, 500, 600, 700, 800, 999};
    public int[] _specialCooldownPrices { get; private set; } = {50, 100, 250, 450, 700};
    public bool _specialAttackUnlocked { get; set; } = false;
    public bool _slideUnlocked { get; set; } = false;

    //General
    public int _revengePointsAmount { get; set; } = 9999;
    public int _level { get; set; } = 1;

    //Texts
    private TextMeshProUGUI level;
    private TextMeshProUGUI price;

    private void Awake() {
        LoadGame();
    }

    public void SaveGame() {
        GameObject saveIcon = GameObject.Find("SavingICon");
        if(saveIcon != null) saveIcon.GetComponent<Animator>().SetTrigger("Save");

        SaveSystem.SaveGame(this);
    }

    public void SaveGameWithoutFeedback() {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame() {
        GameData data = SaveSystem.LoadGame();

        if(data == null) {
            if(SceneManager.GetActiveScene().name == "MainMenu") {
                GameObject.Find("BTNNewGame").SetActive(false);
                GameObject.Find("TextContinuar").GetComponent<TextMeshProUGUI>().text = "Novo Jogo";
                return;
            }
            else return;
        }

        _lifeUpgradeLevel = data._lifeUpgradeLevel;
        _healBottlesUpgradeLevel = data._healBottlesUpgradeLevel;
        _healAmountUpgradeLevel = data._healAmountUpgradeLevel;
        _damageUpgradeLevel = data._damageUpgradeLevel;
        _specialDamageUpgradeLevel = data._specialDamageUpgradeLevel;
        _specialCooldownUpgradeLevel = data._specialCooldownUpgradeLevel;
        _revengePointsAmount = data._revengePointsAmount;
        _specialAttackUnlocked = data._specialAttackUnlocked;
        _slideUnlocked = data._slideUnlocked;
        _level = data._level;
    }

    private void FindTextElements(string name) {
        GameObject life = GameObject.Find(name);
        level = life.transform.Find("Image").Find("Level").GetComponent<TextMeshProUGUI>();
        price = life.transform.Find("LeftSide").Find("RevengePoints").Find("Cost").GetComponent<TextMeshProUGUI>();
    }

    private bool UpdateRevengePoints(int amount) {
        if(_revengePointsAmount - amount >= 0) {
            TextMeshProUGUI revengePoints = GameObject.Find("RevengeQuantity").GetComponent<TextMeshProUGUI>();

            _revengePointsAmount -= amount;
            revengePoints.text = _revengePointsAmount.ToString("0000");
            return true;
        } else return false;
    }

    private void SetMaxLevel() {
        level.text = "MAX";
        level.fontSize = 32;
        price.text = "---";
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.SetActive(false);
    }

    // Update Functions
    public void UpdateLife() {
        FindTextElements("Life");

        if(_lifeUpgradeLevel < _lifePrices.Length && UpdateRevengePoints(_lifePrices[_lifeUpgradeLevel])) {
            _lifeUpgradeLevel++;

            if(_lifeUpgradeLevel < _lifePrices.Length) {
                level.text = (_lifeUpgradeLevel + 1).ToString();
                price.text = _lifePrices[_lifeUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_lifePrices[_lifeUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }
    
    public void UpdateHealBottle() {
        FindTextElements("Potions");

        if(_healBottlesUpgradeLevel < _healBottlesPrices.Length && UpdateRevengePoints(_healBottlesPrices[_healBottlesUpgradeLevel])) {
            _healBottlesUpgradeLevel++;

            //Unlocking Regeneration upgrade option
            if(_healBottlesUpgradeLevel == 1) {
                GameObject.Find("Buttons").transform.Find("Regen").gameObject.SetActive(true);
                GameObject.Find("Regen-Locked").SetActive(false);
            }

            if(_healBottlesUpgradeLevel < _healBottlesPrices.Length) {
                level.text = (_healBottlesUpgradeLevel).ToString();
                price.text = _healBottlesPrices[_healBottlesUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_healBottlesPrices[_healBottlesUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }
    
    public void UpdateHealAmount() {
        FindTextElements("Regen");

        if(_healAmountUpgradeLevel < _healAmountPrices.Length && UpdateRevengePoints(_healAmountPrices[_healAmountUpgradeLevel])) {
            _healAmountUpgradeLevel++;

            if(_healAmountUpgradeLevel < _healAmountPrices.Length) {
                level.text = (_healAmountUpgradeLevel + 1).ToString();
                price.text = _healAmountPrices[_healAmountUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_healAmountPrices[_healAmountUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }
    
    public void UpdateDamage() {
        FindTextElements("Damage");

        if(_damageUpgradeLevel < _damagePrices.Length && UpdateRevengePoints(_damagePrices[_damageUpgradeLevel])) {
            _damageUpgradeLevel++;

            if(_damageUpgradeLevel < _damagePrices.Length) {
                level.text = (_damageUpgradeLevel + 1).ToString();
                price.text = _damagePrices[_damageUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_damagePrices[_damageUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }

    public void UpdateSpecialDamage() {
        FindTextElements("SpecialDamage");

        if(_specialDamageUpgradeLevel < _specialDamagePrices.Length && UpdateRevengePoints(_specialDamagePrices[_specialDamageUpgradeLevel])) {
            _specialDamageUpgradeLevel++;

            if(_specialDamageUpgradeLevel < _specialDamagePrices.Length) {
                level.text = (_specialDamageUpgradeLevel + 1).ToString();
                price.text = _specialDamagePrices[_specialDamageUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_specialDamagePrices[_specialDamageUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }
    
    public void UpdateSpecialCooldown() {
        FindTextElements("SpecialCooldown");

        if(_specialCooldownUpgradeLevel < _specialCooldownPrices.Length && UpdateRevengePoints(_specialCooldownPrices[_specialCooldownUpgradeLevel])) {
            _specialCooldownUpgradeLevel++;

            if(_specialCooldownUpgradeLevel < _specialCooldownPrices.Length) {
                level.text = (_specialCooldownUpgradeLevel + 1).ToString();
                price.text = _specialCooldownPrices[_specialCooldownUpgradeLevel].ToString("000");
            } else SetMaxLevel();
        }
        else if(!UpdateRevengePoints(_specialCooldownPrices[_specialCooldownUpgradeLevel])) GameObject.Find("RevengeQuantity").GetComponent<Animator>().SetTrigger("NotEnoughtPoints");
    }

    public void FinishUpdate() {
        SaveGame();
        StartCoroutine(LoadLevel(1));
    }

    public void NewGame() {
        SaveSystem.DeleteSave();
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int level) {
        GameObject ascend = GameObject.Find("Ascend");
        if(ascend != null) {
            ascend.GetComponent<Animator>().SetTrigger("Respawn");
            yield return new WaitForSeconds(1);
        }

        SceneManager.LoadScene("Level " + level);
    }
}