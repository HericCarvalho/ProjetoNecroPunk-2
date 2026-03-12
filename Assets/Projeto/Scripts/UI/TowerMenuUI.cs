using UnityEngine;

public class TowerMenuUI : MonoBehaviour
{
    private TowerUpgrade currentTower;

    public void Open(TowerUpgrade tower)
    {
        currentTower = tower;

        gameObject.SetActive(true);
    }

    public void Upgrade()
    {
        currentTower.Upgrade();
    }

    public void Sell()
    {
        currentTower.Sell();
    }
}