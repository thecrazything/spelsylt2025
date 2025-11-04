using UnityEngine;

public class ToggleBehaviour : MonoBehaviour, IControl
{
    [SerializeField] private bool _isOn = false;
    [SerializeField] private KnobBehaviour.RotationAxis _rotationAxis = KnobBehaviour.RotationAxis.Y;
    [SerializeField] private float _rotationSpeed = 180f; // degrees per second
    [SerializeField] private float _rotationDistance = 90f; // degrees to spin to turn on

    private float _currentRotation = 0f;
    private bool _isSpinning = false;
    private bool _spinToOn = false;

    void Start()
    {
        SetRotation(0f);
    }

    void Update()
    {
        if (_isSpinning)
        {
            float target = _spinToOn ? _rotationDistance : 0f;

            // Move towards target rotation
            _currentRotation = Mathf.MoveTowards(_currentRotation, target, _rotationSpeed * Time.deltaTime);
            SetRotation(_currentRotation);

            // Check if reached target
            if (_spinToOn && Mathf.Approximately(_currentRotation, _rotationDistance))
            {
                _isOn = true;
                _isSpinning = false;
            }
            else if (!_spinToOn && Mathf.Approximately(_currentRotation, 0f))
            {
                _isOn = false;
                _isSpinning = false;
            }
        }
    }

    public void OnDrag(float deltaX, float deltaY)
    {
        // Not used for this control
    }

    public float GetValue()
    {
        return _isOn ? 1f : 0f;
    }

    public void OnMousePressStart()
    {
        _isSpinning = true;
        _spinToOn = true;
    }

    public void OnMousePressEnd()
    {
        _isSpinning = true;
        _spinToOn = false;
    }

    private void SetRotation(float angle)
    {
        Vector3 euler = transform.localEulerAngles;
        switch (_rotationAxis)
        {
            case KnobBehaviour.RotationAxis.X:
                euler.x = angle;
                break;
            case KnobBehaviour.RotationAxis.Y:
                euler.y = angle;
                break;
            case KnobBehaviour.RotationAxis.Z:
                euler.z = angle;
                break;
        }
        transform.localRotation = Quaternion.Euler(euler);
    }
}