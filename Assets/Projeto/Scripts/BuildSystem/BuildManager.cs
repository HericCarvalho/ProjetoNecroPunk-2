using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public TowerData selectedTower;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SelectTower(TowerData tower)
    {
        selectedTower = tower;

        TowerPreview.instance.CreatePreview(tower.prefab);
    }

    public bool CanBuild()
    {
        return selectedTower != null;
    }

    public void CancelBuild()
    {
        selectedTower = null;
    }
}