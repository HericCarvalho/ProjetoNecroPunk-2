using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    public int moneyReward = 10;
    public int restosReward = 2;

    public void GiveReward()
    {
        PlayerResources.instance.AddMoney(moneyReward);
        PlayerResources.instance.AddRestos(restosReward);
    }
}