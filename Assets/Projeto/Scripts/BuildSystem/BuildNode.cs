using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public GameObject tower;

    public void BuildTower(TowerData data)
    {
        if (tower != null)
            return;

        if (!PlayerResources.instance.CanAfford(data.costMoney, data.costRestos))
        {
            Debug.Log("Sem recursos");
            return;
        }

        PlayerResources.instance.Spend(data.costMoney, data.costRestos);

        Debug.Log("Tentando construir torre");
        tower = Instantiate(data.prefab, transform.position, Quaternion.identity);
    }
}