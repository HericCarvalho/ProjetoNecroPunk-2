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
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);

            Debug.Log("Registrou: " + enemy.name);
        }
    }

    public void UnregisterEnemy(Transform enemy)
    {
        enemies.Remove(enemy);

        Debug.Log("Removeu: " + enemy.name);
    }
    public void Cleanup()
    {
        enemies.RemoveAll(e =>
            e == null ||
            !e.gameObject.activeInHierarchy
        );
    }

    public int GetAliveEnemies()
    {
        return enemies.Count;
    }
}