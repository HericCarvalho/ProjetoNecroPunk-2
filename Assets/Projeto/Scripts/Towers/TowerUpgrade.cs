using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    private Tower tower;

    void Awake()
    {
        tower = GetComponent<Tower>();
    }

    public void Upgrade()
    {
        if (tower == null) return;
        if (tower.data == null) return;
        if (tower.data.nextUpgrade == null) return;

        TowerData next = tower.data.nextUpgrade;

        if (!PlayerResources.instance.CanAfford(
            next.upgradeCostMoney,
            next.upgradeCostRestos))
            return;

        PlayerResources.instance.Spend(
            next.upgradeCostMoney,
            next.upgradeCostRestos);

        Instantiate(next.prefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void Sell()
    {
        if (tower == null || tower.data == null) return;

        PlayerResources.instance.AddMoney(tower.data.sellValue);

        Destroy(gameObject);
    }
}