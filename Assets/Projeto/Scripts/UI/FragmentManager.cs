using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager instance;

    private Dictionary<string, int> fragments = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }

    public void AddFragment(string enemyID)
    {
        if (!fragments.ContainsKey(enemyID))
            fragments[enemyID] = 0;

        fragments[enemyID]++;

    }

    public int GetFragments(string enemyID)
    {
        if (!fragments.ContainsKey(enemyID))
            return 0;

        return fragments[enemyID];
    }

    public bool CanRevive(string enemyID)
    {
        return GetFragments(enemyID) >= 5;
    }

    public void ConsumeFragments(string enemyID, int amount)
    {
        if (!fragments.ContainsKey(enemyID))
            return;

        fragments[enemyID] -= amount;

        if (fragments[enemyID] < 0)
            fragments[enemyID] = 0;
    }
}