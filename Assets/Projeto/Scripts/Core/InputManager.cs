using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // TOUCH (mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 pos = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleClick(pos);
        }

        // MOUSE (PC testing)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            HandleClick(pos);
        }
    }

    void HandleClick(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.collider.name);

            BuildNode node = hit.collider.GetComponentInParent<BuildNode>();

            if (node == null)
                return;

            BuildMenuUI.instance.OpenMenu(node);
        }
    }
}