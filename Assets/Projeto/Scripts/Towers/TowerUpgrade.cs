using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    TowerStats stats;

    void Awake()
    {
        stats = GetComponent<TowerStats>();
    }

    public void Upgrade()
    {
        if (stats.data.nextUpgrade == null)
            return;

        TowerData next = stats.data.nextUpgrade;

        if (!PlayerResources.instance.CanAfford(next.upgradeCostMoney, next.upgradeCostRestos))
            return;

        PlayerResources.instance.Spend(next.upgradeCostMoney, next.upgradeCostRestos);

        Instantiate(next.prefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void Sell()
    {
        PlayerResources.instance.AddMoney(stats.data.sellValue);

        Destroy(gameObject);
    }
}