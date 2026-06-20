using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public bool isOccupied;
    public Tower currentTower;

    void Update()
    {
        if (currentTower == null)
            Debug.Log("Sem torre");

        else
            Debug.Log(currentTower.name);
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