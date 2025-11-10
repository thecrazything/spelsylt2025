using TMPro;
using UnityEngine;

public class CountdownClock : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float countdownTime = 60f;

    public Light light;

    public bool isOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
            text.text = "";
        }
    }
}
