using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TD/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Base")]
    public string towerName;
    public GameObject prefab;

    [Header("Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public float damage = 50f;

    [Header("Economy")]
    public int costMoney = 50;
    public int costRestos = 0;
    public int sellValue = 25;

    [Header("XP")]
    public int baseXPToLevel = 10;

    [Header("Upgrade")]
    public TowerData nextUpgrade;
    public int upgradeCostMoney = 100;
    public int upgradeCostRestos = 0;
}