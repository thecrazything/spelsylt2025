using UnityEngine;

public class TestRadio : MonoBehaviour
{
    public KnobBehaviour frequencyControl;
    public SliderBehaviour amplitudeControl;
    public KnobBehaviour typeControl;

    public WaveformController waveformController;

    public IndicatorLight frequencyMatchLight;
    public IndicatorLight amplitudeMatchLight;
    public IndicatorLight typeMatchLight;
    public IndicatorLight offsetXMatchLight;
    public IndicatorLight offsetYMatchLight;
    public ToggleBehaviour key;

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

        if (key.GetValue() > 0f)
        {
            TurnOnMatcherLights();
        }
        else
        {
            TurnOffMatcherLights();
        }
    }

    public void TurnOnMatcherLights()
    {
        WaveFormModel testModel = GetTempWaveformModel();
        WaveFormModel targetModel = GameManager.Instance.GetClosestMatch(testModel);

        bool[] matches = testModel.Matches(targetModel);

        frequencyMatchLight.SetIsOn(matches[0]);
        amplitudeMatchLight.SetIsOn(matches[1]);
        typeMatchLight.SetIsOn(matches[2]);
        offsetXMatchLight.SetIsOn(matches[3]);
        offsetYMatchLight.SetIsOn(matches[4]);
    }

    public void TurnOffMatcherLights()
    {
        frequencyMatchLight.SetIsOn(false);
        amplitudeMatchLight.SetIsOn(false);
        typeMatchLight.SetIsOn(false);
        offsetXMatchLight.SetIsOn(false);
        offsetYMatchLight.SetIsOn(false);
    }
    
    private WaveFormModel GetTempWaveformModel()
    {
        WaveFormModel model = new WaveFormModel();
        model.Frequency = frequencyControl.GetValue();
        model.Amplitude = amplitudeControl.GetValue();
        model.Type = (int)typeControl.GetValue();
        // TODO add x and y
        return model;
    }
}
