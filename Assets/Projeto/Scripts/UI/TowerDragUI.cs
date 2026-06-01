using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TowerData towerData;

    RectTransform rectTransform;
    Canvas canvas;
    Vector2 originalPosition;

    GameObject ghost;
    BuildNode currentNode;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentNode = BuildMenuUI.instance.GetNode();

        if (currentNode == null) return;

        ghost = Instantiate(towerData.prefab);
        ghost.transform.position = currentNode.transform.position;

        SetGhostMaterial(ghost);

        Tower t = ghost.GetComponent<Tower>();
        if (t != null)
            t.isPreview = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentNode != null &&
            BuildManager.instance.CanBuildOn(currentNode, towerData))
        {
            BuildManager.instance.BuildOn(currentNode, towerData);
        }

        if (ghost != null)
            Destroy(ghost);

        rectTransform.anchoredPosition = originalPosition;

        BuildMenuUI.instance.CloseMenu();
    }

    void SetGhostMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                if (m.HasProperty("_Color"))
                {
                    Color c = m.GetColor("_Color");
                    c.a = 0.7f;
                    m.SetColor("_Color", c);
                }
                else if (m.HasProperty("_BaseColor"))
                {
                    Color c = m.GetColor("_BaseColor");
                    c.a = 0.7f;
                    m.SetColor("_BaseColor", c);
                }
            }
        }
    }
}