using System.Collections.Generic;
using UnityEngine;

public class TestRadio : MonoBehaviour
{
    public KnobBehaviour frequencyControl;
    public KnobBehaviour amplitudeControl;
    public KnobBehaviour typeControl;

    public WaveformController waveformController;

    public SliderBehaviour offsetXControl;
    public SliderBehaviour offsetYControl;

    public IndicatorLight frequencyMatchLight;
    public IndicatorLight amplitudeMatchLight;
    public IndicatorLight typeMatchLight;
    public IndicatorLight offsetXMatchLight;
    public IndicatorLight offsetYMatchLight;

    public IndicatorLight powerLight;

    public ToggleBehaviour key;

    public ButtonBehaviour powerButton;
    public ButtonBehaviour matcherButton;

    public IndicatorLight matcherLight;

    public RadioSound sound;

    public Light screenLightA;
    public Light screenLightB;

    public ScreenBackground screenBackground;

    public bool IsOn = true;

    public TubeLLight tubeLight1;
    public TubeLLight tubeLight2;
    public TubeLLight tubeLight3;
    public TubeLLight tubeLight4;
    public TubeLLight tubeLight5;
    public TubeLLight tubeLight6;
    public TubeLLight tubeLight7;
    public TubeLLight tubeLight8;

    public int?[] tubeLightTexts = new int?[]
    {
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
    };

    private int _tubeLightIndex = 0;
    private List<TubeLLight> _tubeLights = new List<TubeLLight>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matcherLight = matcherButton.GetComponent<IndicatorLight>();
        _tubeLights.Add(tubeLight1);
        _tubeLights.Add(tubeLight2);
        _tubeLights.Add(tubeLight3);
        _tubeLights.Add(tubeLight4);
        _tubeLights.Add(tubeLight5);
        _tubeLights.Add(tubeLight6);
        _tubeLights.Add(tubeLight7);
        _tubeLights.Add(tubeLight8);
    }

    // Update is called once per frame
    void Update()
    {
        if (powerButton.GetValue() > 0f)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
        if (!IsOn)
        {
            return;
        }
        waveformController.SetFrequency(frequencyControl.GetValue());
        waveformController.SetAmplitude(amplitudeControl.GetValue());
        waveformController.SetType((int)typeControl.GetValue());
        waveformController.SetXOffset(offsetXControl.GetValue());
        waveformController.SetYOffset(offsetYControl.GetValue());

        WaveFormModel testModel = GetTempWaveformModel();
        TurnOnMatcherLights(testModel);
        sound.model = testModel;

        WaveFormModel targetModel = GameManager.Instance.GetClosestMatch(testModel);
        if (targetModel == null)
            return;
        bool[] matches = testModel.Matches(targetModel);
        if (matches[0] && matches[1] && matches[2] && matches[3] && matches[4])
        {
            matcherLight.SetIsOn(true);
            if (matcherButton.GetValue() > 0f)
            {
                int solution = GameManager.Instance.ClearWaveform(targetModel);
                TubeLLight tubeLight = _tubeLights[_tubeLightIndex];
                tubeLightTexts[_tubeLightIndex] = solution; // Store the solution for this tube light so we can restore when we turn on
                tubeLight.SetNumber(solution);
                _tubeLightIndex++;
            }
        }
        else
        {
            matcherLight.SetIsOn(false);
        }
    }

    public void TurnOff()
    {
        if (!IsOn)
            return;
        TurnOffMatcherLights();
        IsOn = false;
        sound.enabled = false;
        waveformController.gameObject.SetActive(false);
        GameManager.Instance.TurnOffAllWaveforms();
        powerLight.SetIsOn(false);
        matcherLight.SetIsOn(false);
        screenLightA.enabled = false;
        screenLightB.enabled = false;
        screenBackground.SetIsOn(false);
        tubeLight1.TurnOff();
        tubeLight2.TurnOff();
        tubeLight3.TurnOff();
        tubeLight4.TurnOff();
        tubeLight5.TurnOff();
        tubeLight6.TurnOff();
        tubeLight7.TurnOff();
        tubeLight8.TurnOff();
    }

    public void TurnOn()
    {
        if (IsOn)
            return;
        IsOn = true;
        sound.enabled = true;
        waveformController.gameObject.SetActive(true);
        GameManager.Instance.TurnOnAllWaveforms();
        powerLight.SetIsOn(true);
        screenLightA.enabled = true;
        screenLightB.enabled = true;
        screenBackground.SetIsOn(true);

        for (int i = 0; i < 8; i++)
        {
            TubeLLight tubeLight = _tubeLights[i];
            if (tubeLight != null)
            {
                if (tubeLightTexts[i] == null)
                {
                    tubeLight.TurnOff();
                }
                else
                {
                    tubeLight.SetNumber(tubeLightTexts[i].Value);
                }
            }
        }
    }

    public void TurnOnMatcherLights(WaveFormModel testModel)
    {
        WaveFormModel targetModel = GameManager.Instance.GetClosestMatch(testModel);

        bool[] matches = testModel.Matches(targetModel);

        frequencyMatchLight.SetState(matches[0]);
        amplitudeMatchLight.SetState(matches[1]);
        typeMatchLight.SetState(matches[2]);
        offsetXMatchLight.SetState(matches[3]);
        offsetYMatchLight.SetState(matches[4]);
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
        model.XOffset = offsetXControl.GetValue();
        model.YOffset = offsetYControl.GetValue();
        return model;
    }
}
