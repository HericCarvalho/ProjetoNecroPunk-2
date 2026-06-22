using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        instance = this;
    }
    public GameObject GetObject(GameObject prefab)
    {
        return GetObject(prefab, Vector3.zero, true);
    }

    public GameObject GetObject(GameObject prefab, Vector3 spawnPos, bool autoActivate = true)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = pools[prefab];

        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject objToUse = pool.Dequeue();

        objToUse.transform.position = spawnPos;

        ResetObject(objToUse);

        if (autoActivate)
            objToUse.SetActive(true);

        return objToUse;
    }
    private void ResetObject(GameObject obj)
    {
        var move = obj.GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.enabled = true;
            move.state = EnemyState.Moving;
        }

        var health = obj.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetTarget(null);
        }

        if (move != null)
        {
            move.SendMessage("ResetEnemy", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void ReturnObject(GameObject obj, GameObject prefab)
    {
        if (obj == null || prefab == null)
        {
            Debug.LogError("ReturnObject recebeu NULL!");
            return;
        }

        obj.transform.position = new Vector3(0, -1000, 0);

        var move = obj.GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.state = EnemyState.Moving;
        }

        var health = obj.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetTarget(null);
        }

        obj.SetActive(false);

        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        pools[prefab].Enqueue(obj);
    }
}