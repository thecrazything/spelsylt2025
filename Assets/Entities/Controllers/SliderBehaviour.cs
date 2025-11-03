using System;
using UnityEngine;

public class SliderBehaviour : MonoBehaviour, IControl
{

    [SerializeField] private float _minValue = 0f;
    [SerializeField] private float _maxValue = 1f;
    [SerializeField] private float _currentValue = 0f;
    [SerializeField] private SliderAxis _sliderAxis = SliderAxis.X;
    [SerializeField] private float _sliderSpeed = 0.01f;
    [SerializeField] private float _sliderLength = 1f;

    private Vector3 _origin;

    void Start()
    {
        _origin = transform.localPosition;
    }

    public void OnDrag(float deltaX, float deltaY)
    {
        float delta = (_sliderAxis == SliderAxis.X) ? deltaX : deltaY;

        _currentValue += delta * Time.deltaTime * _sliderSpeed;
        _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);

        float normalizedValue = Mathf.Lerp(0f, _sliderLength, (_currentValue - _minValue) / (_maxValue - _minValue));
        Vector3 localPosition = new Vector3(_origin.x, _origin.y, _origin.z);
        if (_sliderAxis == SliderAxis.X)
        {
            localPosition.x = normalizedValue;
        }
        else
        {
            localPosition.y = normalizedValue;
        }
        transform.localPosition = localPosition;
    }

    public float GetValue()
    {
        return _currentValue;
    }

    [Serializable]
    public enum SliderAxis
    {
        X,
        Y
    }
}