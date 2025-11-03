using UnityEngine;
using UnityEngine.InputSystem;

public class SittingPlayerController : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _headMoveSensitivity = 0.5f;
    [SerializeField] private float _maxHeadAngle = 10f;

    private bool _lockedCursor = false;

    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (_lockedCursor) return;
        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 offset = mousePos - screenCenter;

        Vector2 normalized = new Vector2(
            Mathf.Clamp(offset.x / (Screen.width / 2f), -1f, 1f),
            Mathf.Clamp(offset.y / (Screen.height / 2f), -1f, 1f)
        );

        float xRot = -normalized.y * _maxHeadAngle * _headMoveSensitivity;
        float yRot = normalized.x * _maxHeadAngle * _headMoveSensitivity;

        _playerCamera.transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
    }

    public void SetLocked(bool locked)
    {
        _lockedCursor = locked;
        Cursor.visible = !locked;
    }
}