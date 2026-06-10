using UnityEngine;
using UnityEngine.InputSystem;

public class MobileCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.01f;

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
        // Se o menu estiver aberto, ignora o toque
        if (BuildMenuUI.instance != null && BuildMenuUI.instance.IsMenuOpen)
            return;

        HandlePointerMovement();
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

        Vector3 move = (-right * delta.x + -forward * delta.y) * moveSpeed;

        transform.position += move;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}