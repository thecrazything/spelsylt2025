using System.Collections.Generic;
using UnityEngine;

public class WaveFormModel
{
    public float Frequency { get; set; }
    public float Amplitude { get; set; }
    public int Type { get; set; } // 0 = Sine, 1 = Square, 2 = Triangle, 3 = Sawtooth
    public float XOffset { get; set; }
    public float YOffset { get; set; }

    public bool[] Matches(WaveFormModel other)
    {
        const float epsilon = 0.05f; // TODO adjust as needed for matching accuracy
        return new bool[]
        {
            Mathf.Abs(Frequency - other.Frequency) < epsilon,
            Mathf.Abs(Amplitude - other.Amplitude) < epsilon,
            Type == other.Type,
            Mathf.Abs(XOffset - other.XOffset) < epsilon,
            Mathf.Abs(YOffset - other.YOffset) < epsilon
        };
    }

    public static WaveFormModel GetRandom()
    {
        Random.Range(0.1f, 5.0f);
        return new WaveFormModel
        {
            Frequency = Random.Range(0.1f, 5.0f),
            Amplitude = Random.Range(0.0f, 0.5f),
            Type = Random.Range(0, 4),
            XOffset = Random.Range(-0.25f, 0.25f),
            YOffset = Random.Range(-0.25f, 0.25f)
        };
    }

    public static List<WaveFormModel> GetRandom(int count)
    {
        List<WaveFormModel> models = new List<WaveFormModel>();
        for (int i = 0; i < count; i++)
        {
            models.Add(GetRandom());
        }
        return models;
    }
}