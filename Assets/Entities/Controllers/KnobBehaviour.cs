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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnDrag(0, 0); // Zero out the rotation so it is in sync
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(float deltaX, float deltaY)
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
    
    [Serializable]
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
}
