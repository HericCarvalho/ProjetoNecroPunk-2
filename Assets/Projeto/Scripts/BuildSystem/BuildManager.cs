using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public TowerData selectedTower;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        selectedTower = null;

    }

    public void SelectTower(TowerData tower)
    {
        selectedTower = tower;


    }

    public bool CanBuildOn(BuildNode node, TowerData tower)
    {


        if (node == null) return false;
        if (tower == null) return false;
        if (!node.CanBuild()) return false;

        if (PlayerResources.instance == null) return false;

        return PlayerResources.instance.CanAfford(tower.costMoney, 0);
    }

    public void BuildOn(BuildNode node, TowerData tower)
    {
        if (!CanBuildOn(node, tower))
            return;

        PlayerResources.instance.Spend(tower.costMoney, 0);

        node.BuildTower(tower);
    }

    public void CancelBuild()
    {
        selectedTower = null;
    }
}