using UnityEngine;

public class ButtonBehaviour : MonoBehaviour, IControl
{
    [SerializeField] private bool _isToggle = false;
    public bool IsPressed = false;
    private bool _pressedThisFrame = false;

    public void OnDrag(float deltaX, float deltaY)
    {
        // Buttons do not respond to drag events
    }

    public float GetValue()
    {
        if (_isToggle)
        {
            return IsPressed ? 1f : 0f;
        }
        else
        {
            return _pressedThisFrame ? 1f : 0f;
        }
    }

    public void OnMousePressStart()
    {
        if (_isToggle)
        {
            IsPressed = !IsPressed;
        }
        else
        {
            if (!IsPressed)
            {
                IsPressed = true;
                _pressedThisFrame = true;
            }
        }
    }

    public void OnMousePressEnd()
    {
        if (!_isToggle)
        {
            IsPressed = false;
        }
    }

    public void ResetFrame()
    {
        _pressedThisFrame = false;
    }
}