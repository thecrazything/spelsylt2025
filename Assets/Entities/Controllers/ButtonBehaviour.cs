using UnityEngine;

public class ButtonBehaviour : MonoBehaviour, IControl
{
    [SerializeField] private bool _isToggle = false;
    public bool IsPressed = false;
    public void OnDrag(float deltaX, float deltaY)
    {
        // Buttons do not respond to drag events
    }

    public float GetValue()
    {
        return IsPressed ? 1f : 0f;
    }

    public void OnMousePressStart()
    {
        if (_isToggle)
        {
            IsPressed = !IsPressed;
            return;
        }
        else
        {
            IsPressed = true;
        }
    }

    public void OnMousePressEnd()
    {
        if (!_isToggle)
        {
            IsPressed = false;
        }
    }
}