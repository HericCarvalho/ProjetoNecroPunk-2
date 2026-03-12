using UnityEngine;

public class EnemyRegistration : MonoBehaviour
{
    void OnEnable()
    {
        EnemyManager.instance.RegisterEnemy(transform);
    }

    void OnDisable()
    {
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(transform);
        }
    }
}