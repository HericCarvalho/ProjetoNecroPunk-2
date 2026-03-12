using UnityEngine;
using UnityEngine.InputSystem;

public class MobileCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.01f;

    [Header("Zoom")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 10f;
    public float maxZoom = 40f;

    [Header("Map Limits")]
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    [Header("Drag Settings")]
    public float dragThreshold = 10f;

    private Vector2 startPointerPosition;
    private Vector2 lastPointerPosition;

    private bool isDragging = false;

    void Update()
    {
        if (BuildMenuUI.instance != null && BuildMenuUI.instance.IsMenuOpen)
            return;

        HandlePointerMovement();
        HandleZoom();
        ClampPosition();
    }

    void HandlePointerMovement()
    {
        if (Pointer.current == null)
            return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            startPointerPosition = Pointer.current.position.ReadValue();
            lastPointerPosition = startPointerPosition;
            isDragging = false;
        }

        if (Pointer.current.press.isPressed)
        {
            Vector2 currentPosition = Pointer.current.position.ReadValue();

            float distance = Vector2.Distance(startPointerPosition, currentPosition);

            if (!isDragging && distance > dragThreshold)
            {
                isDragging = true;
            }

            if (isDragging)
            {
                Vector2 delta = currentPosition - lastPointerPosition;

                MoveCamera(delta);

                lastPointerPosition = currentPosition;
            }
        }

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    void MoveCamera(Vector2 delta)
    {
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        forward.y = 0;
        forward.Normalize();

        Vector3 move =
            (-right * delta.x + -forward * delta.y) * moveSpeed;

        transform.position += move;
    }

    void HandleZoom()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll != 0)
            {
                Zoom(scroll * zoomSpeed);
            }
        }

        if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touch0 = Touchscreen.current.touches[0];
            var touch1 = Touchscreen.current.touches[1];

            if (touch0.press.isPressed && touch1.press.isPressed)
            {
                Vector2 pos0 = touch0.position.ReadValue();
                Vector2 pos1 = touch1.position.ReadValue();

                float currentDistance = Vector2.Distance(pos0, pos1);

                Vector2 prev0 = pos0 - touch0.delta.ReadValue();
                Vector2 prev1 = pos1 - touch1.delta.ReadValue();

                float prevDistance = Vector2.Distance(prev0, prev1);

                float difference = currentDistance - prevDistance;

                Zoom(difference * zoomSpeed);
            }
        }
    }

    void Zoom(float increment)
    {
        Vector3 pos = transform.position;

        pos.y -= increment;

        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);

        transform.position = pos;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}