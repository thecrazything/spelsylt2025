using UnityEngine;

public class IndicatorLight : MonoBehaviour
{
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private Color onColor = Color.green;
    private Renderer lightRenderer;
    [SerializeField] private bool _isOn = false;
    private bool _state = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightRenderer = GetComponent<Renderer>();
        if (_isOn)
            lightRenderer.material.SetColor("_EmissionColor", _state ? onColor : offColor);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetIsOn(bool isOn)
    {
        _isOn = isOn;
        if (_isOn)
        {
            lightRenderer.material.SetColor("_EmissionColor", _state ? onColor : offColor);
        }
        else
        {
            lightRenderer.material.SetColor("_EmissionColor", Color.black);
        }
    }
    
    public void SetState(bool state)
    {
        _isOn = true;
        _state = state;
        if (lightRenderer != null)
        {
            lightRenderer.material.SetColor("_EmissionColor", _state ? onColor : offColor);
        }
    }
}
