using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;

    [Header("UI")]
    public GameObject necroUI;

    private GameObject unitToPlace;

    private bool justStartedPlacing = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (unitToPlace == null)
            return;

        if (justStartedPlacing)
        {
            justStartedPlacing = false;
            return;
        }

        if (IsPointerPressed() && !IsPointerOverUI())
        {
            PlaceUnit();
        }
    }

    public void StartPlacing(GameObject prefab)
    {
        Debug.Log("StartPlacing chamado");

        if (prefab == null)
        {
            Debug.LogError("Prefab NULL");
            return;
        }

        unitToPlace = prefab;

        Debug.Log("Prefab armazenado: " + prefab.name);

        justStartedPlacing = true;

        if (necroUI != null)
            necroUI.SetActive(false);
    }

    void PlaceUnit()
    {
        Vector3 pos = GetPointerPosition();

        if (pos == Vector3.zero)
            return;

        GameObject unit =
    ObjectPool.instance.GetObject(unitToPlace);

        unit.transform.position = pos;

        RevivedUnit revived =
            unit.GetComponent<RevivedUnit>();

        if (revived != null)
        {
            revived.SetHomePosition(pos);
        }

        unitToPlace = null;

        if (necroUI != null)
            necroUI.SetActive(true);
    }

    bool IsPointerPressed()
    {
        // Mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        // PC
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    Vector3 GetPointerPosition()
    {
        Vector2 screenPos = Vector2.zero;

        // Mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // PC
        else if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}