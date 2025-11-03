using UnityEngine;

public class WaveformController : MonoBehaviour
{

    [SerializeField] private Renderer _screen;

    private Material _screenWaveformMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _screenWaveformMaterial = _screen.material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAmplitude(float amplitude)
    {
        if (_screenWaveformMaterial != null)
        {
            _screenWaveformMaterial.SetFloat("_WaveAmplitude", amplitude);
        }
    }

    public void SetFrequency(float frequency)
    {
        if (_screenWaveformMaterial != null)
        {
            _screenWaveformMaterial.SetFloat("_WaveFrequency", frequency);
        }
    }

    public void SetType(int type)
    {
        if (type < 0) type = 0;
        if (type > 3) type = 3;
        if (_screenWaveformMaterial != null)
        {
            _screenWaveformMaterial.SetInt("_WaveType", type);
        }
    }

    public void SetXOffset(float xOffset)
    {
        if (_screenWaveformMaterial != null)
        {
            _screenWaveformMaterial.SetFloat("_WaveOffsetX", xOffset);
        }
    }
    
    public void SetYOffset(float yOffset)
    {
        if (_screenWaveformMaterial != null)
        {
            _screenWaveformMaterial.SetFloat("_WaveOffsetY", yOffset);
        }
    }
}
