using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteractionController : MonoBehaviour
{
    private IControl activeControl = null;
    private SittingPlayerController _sittingPlayerController;
    private Vector2 savedCursorPosition;

    void Start()
    {
        _sittingPlayerController = GetComponent<SittingPlayerController>(); 
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<IControl>(out var control))
                {
                    activeControl = control;
                    savedCursorPosition = mousePosition;
                     _sittingPlayerController.SetLocked(true);
                }
            }
        }

        if (activeControl != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float deltaX = mouseDelta.x;
            float deltaY = mouseDelta.y;
            if (Mathf.Abs(deltaX) > Mathf.Epsilon || Mathf.Abs(deltaY) > Mathf.Epsilon)
            {
                activeControl.OnDrag(deltaX, deltaY);
            }
            _sittingPlayerController.SetLocked(true);
        }

        if (activeControl != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            activeControl = null;
            Mouse.current.WarpCursorPosition(savedCursorPosition);
            StartCoroutine(UnlockPlayer()); // wait a frame before unlocking player, prevents flicker
        }
    }

    private IEnumerator UnlockPlayer()
    {
        yield return null;
        _sittingPlayerController.SetLocked(false);
    }
}