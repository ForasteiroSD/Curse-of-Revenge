using UnityEngine;

[System.Serializable]
public class GameData {
    //Player stats
    public int _lifeUpgradeLevel { get; private set; }
    public int _healBottlesUpgradeLevel { get; private set; }
    public int _healAmountUpgradeLevel { get; private set; }
    public int _DamageUpgradeLevel { get; private set; }
    public int _specialCooldownUpgradeLevel { get; private set; }

    //General
    public int _revengePointsAmount { get; private set; }

    public GameData (GameManager gameManager) {
        _lifeUpgradeLevel = gameManager._lifeUpgradeLevel;
        _healBottlesUpgradeLevel = gameManager._healBottlesUpgradeLevel;
        _healAmountUpgradeLevel = gameManager._healAmountUpgradeLevel;
        _DamageUpgradeLevel = gameManager._DamageUpgradeLevel;
        _specialCooldownUpgradeLevel = gameManager._specialCooldownUpgradeLevel;
        _revengePointsAmount = gameManager._revengePointsAmount;
    }
}
