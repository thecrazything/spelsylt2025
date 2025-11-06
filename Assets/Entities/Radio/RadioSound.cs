using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioSound : MonoBehaviour
{
    public WaveFormModel model;

    private float phase;
    private float sampleRate;
    private System.Random noiseRandom;
    private float humPhase;

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        noiseRandom = new System.Random(); // Initialize here
        humPhase = 0f; // Initialize hum phase
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (model == null) return;

        float freq = Mathf.Lerp(60f, 500f, Mathf.InverseLerp(0.1f, 5f, model.Frequency));
        float amp = Mathf.Clamp01(Mathf.InverseLerp(0f, 0.5f, model.Amplitude));
        float phaseOffset = model.XOffset * 2 * Mathf.PI;

        float leftGain = 1;
        float rightGain = 0.5f;

        for (int i = 0; i < data.Length; i += channels)
        {
            float t = phase + phaseOffset;
            float sample = 0f;
            switch (model.Type)
            {
                case 0: sample = Mathf.Sin(t); break;
                case 1: sample = Mathf.Sign(Mathf.Sin(t)); break;
                case 2: sample = Mathf.PingPong(t / Mathf.PI, 1f) * 2f - 1f; break;
                case 3: sample = 2f * (t / (2 * Mathf.PI) - Mathf.Floor(0.5f + t / (2 * Mathf.PI))); break;
            }
            sample *= amp;

            // Add subtle white noise (static)
            float noiseAmount = 0.05f;
            float noise = ((float)noiseRandom.NextDouble() * 2f - 1f) * noiseAmount;
            sample += noise;

            // Add a low 50Hz hum (completely independent)
            float humAmount = 0.2f; // Lower for subtle hum
            float humFreq = 50f;     // Fixed hum frequency

            // Write to each channel and increment humPhase for each sample
            for (int c = 0; c < channels; c++)
            {
                float hum = Mathf.Sin(humPhase) * humAmount;
                float outSample = Mathf.Clamp(sample + hum, -1f, 1f);

                if (c == 0)
                    data[i] = outSample * leftGain;
                else
                    data[i + 1] = outSample * rightGain;

                humPhase += 2 * Mathf.PI * humFreq / sampleRate;
                if (humPhase > 2 * Mathf.PI) humPhase -= 2 * Mathf.PI;
            }

            phase += 2 * Mathf.PI * freq / sampleRate;
            if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;
        }
    }
}