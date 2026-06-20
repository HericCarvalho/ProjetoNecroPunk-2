using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public bool isOccupied;
    public Tower currentTower;

    void Update()
    {

    }
    public bool CanBuild()
    {
        return currentTower == null;
    }

    public bool HasTower()
    {
        return currentTower != null;
    }

    public GameObject BuildTower(TowerData towerData)
    {
        GameObject towerGO = Instantiate(
            towerData.prefab,
            transform.position,
            Quaternion.identity
        );

        towerGO.transform.SetParent(transform);

        currentTower = towerGO.GetComponent<Tower>();
        isOccupied = true;

        return towerGO;
    }
}