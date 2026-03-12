using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public static TowerPreview instance;

    GameObject previewObject;

    public Material validMaterial;
    public Material invalidMaterial;

    Renderer[] renderers;

    void Awake()
    {
        instance = this;
    }

    public void CreatePreview(GameObject towerPrefab)
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(towerPrefab);

        renderers = previewObject.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (previewObject == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            previewObject.transform.position = hit.point;

            BuildNode node = hit.collider.GetComponent<BuildNode>();

            bool valid = node != null && node.tower == null;

            SetColor(valid);
        }
    }

    void SetColor(bool valid)
    {
        foreach (Renderer r in renderers)
        {
            r.material = valid ? validMaterial : invalidMaterial;
        }
    }

    public void CancelPreview()
    {
        if (previewObject != null)
            Destroy(previewObject);
    }
}