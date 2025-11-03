using UnityEngine;

public class TestRadio : MonoBehaviour
{
    public KnobBehaviour frequencyControl;
    public SliderBehaviour amplitudeControl;
    public KnobBehaviour typeControl;

    public WaveformController waveformController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waveformController.SetFrequency(frequencyControl.GetValue());
        waveformController.SetAmplitude(amplitudeControl.GetValue());
        waveformController.SetType((int)typeControl.GetValue());
    }
}
