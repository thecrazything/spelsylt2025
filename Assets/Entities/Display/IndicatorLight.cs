using UnityEngine;

public class IndicatorLight : MonoBehaviour
{
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private Color onColor = Color.green;
    private Renderer lightRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void SetIsOn(bool isOn)
    {
        if (lightRenderer != null)
        {
            lightRenderer.material.SetColor("_EmissionColor", isOn ? onColor : offColor);
        }
    }
}
