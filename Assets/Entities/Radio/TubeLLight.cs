using TMPro;
using UnityEngine;

public class TubeLLight : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNumber(int number)
    {
        if (number < 0) number = 0;
        if (number > 9) number = 9;
        if (_text != null)
        {
            _text.text = number.ToString();
        }
    }
    
    public void TurnOff()
    {
        if (_text != null)
        {
            _text.text = "";
        }
    }
}
