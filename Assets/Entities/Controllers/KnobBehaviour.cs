using System;
using UnityEngine;

public class KnobBehaviour : MonoBehaviour, IControl
{

    [SerializeField] private RotationAxis _rotationAxis = RotationAxis.Y;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _minRotation = -45f;
    [SerializeField] private float _maxRotation = 45f;
    [SerializeField] private float _currentRotationValue = 0f;
    [SerializeField] private float _maxRotationValue = 1f;
    [SerializeField] private float _minRotationValue = 0;
    [SerializeField] private bool _stepped = false; // a stepped knob only moves in discrete steps
    [SerializeField] private int _steps = 5; // number of steps if stepped is true
    private int _currentStep = 0;
    private float _stepAccumulator = 0f;
    [SerializeField] private float _stepThreshold = 0.01f;

    private AudioSource _audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out _audioSource);
        OnDrag(0, 0); // Zero out the rotation so it is in sync
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(float deltaX, float deltaY)
    {
        if (_stepped && _steps > 1)
        {
            int lastStep = _currentStep;
            _stepAccumulator += deltaX * _rotationSpeed * Time.deltaTime;

            if (_stepAccumulator > _stepThreshold)
            {
                if (_currentStep > 0)
                    _currentStep--;
                _stepAccumulator = 0f;
            }
            else if (_stepAccumulator < -_stepThreshold)
            {
                if (_currentStep < _steps)
                    _currentStep++;
                _stepAccumulator = 0f;
            }

            float stepPercentage = (float)_currentStep / (_steps);
            float targetRotation = Mathf.Lerp(_minRotation, _maxRotation, stepPercentage);

            Vector3 euler = transform.localEulerAngles;
            switch (_rotationAxis)
            {
                case RotationAxis.X:
                    euler.x = targetRotation;
                    break;
                case RotationAxis.Y:
                    euler.y = targetRotation;
                    break;
                case RotationAxis.Z:
                    euler.z = targetRotation;
                    break;
            }
            transform.localRotation = Quaternion.Euler(euler);

            // Also update _currentRotationValue for consistency
            _currentRotationValue = Mathf.Lerp(_minRotationValue, _maxRotationValue, stepPercentage);
            if (lastStep != _currentStep && _audioSource != null)
            {
                _audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                _audioSource.Play();
            }
        }
        else
        {
            _currentRotationValue += deltaX * _rotationSpeed * Time.deltaTime;
            _currentRotationValue = Mathf.Clamp(_currentRotationValue, _minRotationValue, _maxRotationValue);

            var rotPercentage = (_currentRotationValue - _minRotationValue) / (_maxRotationValue - _minRotationValue);
            float targetRotation = Mathf.Lerp(_minRotation, _maxRotation, rotPercentage);

            Vector3 euler = transform.localEulerAngles;

            switch (_rotationAxis)
            {
                case RotationAxis.X:
                    euler.x = targetRotation;
                    break;
                case RotationAxis.Y:
                    euler.y = targetRotation;
                    break;
                case RotationAxis.Z:
                    euler.z = targetRotation;
                    break;
            }

            transform.localRotation = Quaternion.Euler(euler);
        }
    }

    public float GetValue()
    {
        return _currentRotationValue;
    }

    public void OnMousePressStart()
    {

    }

    public void OnMousePressEnd()
    {

    }

    [Serializable]
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public void ResetFrame()
    {
        // No per-frame state to reset for this control
    }
}
