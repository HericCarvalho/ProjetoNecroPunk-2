using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public TowerData data;

    [Header("Runtime Stats")]
    public float range;
    public float fireRate;
    public float damage;

    void Awake()
    {
        ApplyData();
    }

    public void ApplyData()
    {
        if (data == null)
            return;

        range = data.range;
        fireRate = data.fireRate;
        damage = data.damage;
    }
}