using UnityEngine;

[System.Serializable]
public class GameData {
    //Player stats
    public int _lifeUpgradeLevel { get; private set; }
    public int _healBottlesUpgradeLevel { get; private set; }
    public int _healAmountUpgradeLevel { get; private set; }
    public int _damageUpgradeLevel { get; private set; }
    public int _specialDamageUpgradeLevel { get; private set; }
    public int _specialCooldownUpgradeLevel { get; private set; }
    public bool _specialAttackUnlocked { get; private set; }
    public bool _slideUnlocked { get; private set; }

    //General
    public int _revengePointsAmount { get; private set; }
    public int _level { get; private set; }

    public GameData (GameManager gameManager) {
        _lifeUpgradeLevel = gameManager._lifeUpgradeLevel;
        _healBottlesUpgradeLevel = gameManager._healBottlesUpgradeLevel;
        _healAmountUpgradeLevel = gameManager._healAmountUpgradeLevel;
        _damageUpgradeLevel = gameManager._damageUpgradeLevel;
        _specialDamageUpgradeLevel = gameManager._specialDamageUpgradeLevel;
        _specialCooldownUpgradeLevel = gameManager._specialCooldownUpgradeLevel;
        _revengePointsAmount = gameManager._revengePointsAmount;
        _specialAttackUnlocked = gameManager._specialAttackUnlocked;
        _slideUnlocked = gameManager._slideUnlocked;
        _level = gameManager._level;
    }
}
