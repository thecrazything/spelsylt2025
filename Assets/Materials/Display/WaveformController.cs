using UnityEngine;

public class WaveformController : MonoBehaviour
{

    [SerializeField] private Renderer _screen;
    [SerializeField] private Color _baseColor = Color.green;

    private Material _screenWaveformMaterial;
    private WaveFormModel _model = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _screenWaveformMaterial = _screen.material;
        _screenWaveformMaterial.SetColor("_BaseColor", _baseColor);
        if (_model != null)
        {
            SetAmplitude(_model.Amplitude);
            SetFrequency(_model.Frequency);
            SetType(_model.Type);
            SetXOffset(_model.XOffset);
            SetYOffset(_model.YOffset);
        }
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

    public void SetWaveform(WaveFormModel waveform)
    {
        _model = waveform;
    }
}
