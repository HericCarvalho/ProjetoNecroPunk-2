using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<Transform> enemies = new List<Transform>();

    void Awake()
    {
        instance = this;
    }

    public void RegisterEnemy(Transform enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(Transform enemy)
    {
        enemies.Remove(enemy);
    }
}