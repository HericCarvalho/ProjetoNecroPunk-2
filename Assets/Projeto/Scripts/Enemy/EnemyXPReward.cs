using UnityEngine;

public class EnemyXPReward : MonoBehaviour
{
    public int xpReward = 5;

    public void GiveXP(GameObject tower)
    {
        Tower towerXP = tower.GetComponent<Tower>();

        if (towerXP != null)
        {
            towerXP.GainXP(xpReward);
        }
    }
}