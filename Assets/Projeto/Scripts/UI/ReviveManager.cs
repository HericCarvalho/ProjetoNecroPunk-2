using UnityEngine;
using UnityEngine.EventSystems;

public class ReviveManager : MonoBehaviour
{
    public static ReviveManager instance;

    void Awake()
    {
        instance = this;
    }

    public void Revive(string enemyID,GameObject revivedPrefab,int manaCost,int fragmentCost)
    {
        if (!FragmentManager.instance.CanRevive(enemyID))
            return;

        if (!PlayerMana.instance.HasMana(manaCost))
            return;

        PlayerMana.instance.SpendMana(manaCost);

        FragmentManager.instance.ConsumeFragments(
            enemyID,
            fragmentCost
        );

        Instantiate(
            revivedPrefab,
            GetSpawnPosition(),
            Quaternion.identity
        );
    }

    Vector3 GetSpawnPosition()
    {
        return Vector3.zero; 
    }
}