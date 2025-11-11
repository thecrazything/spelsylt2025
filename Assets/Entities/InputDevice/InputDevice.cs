using TMPro;
using UnityEngine;

public class InputDevice : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _inputText;
    [SerializeField] private ButtonBehaviour enterButton;
    [SerializeField] private ButtonBehaviour button1;
    [SerializeField] private ButtonBehaviour button2;
    [SerializeField] private ButtonBehaviour button3;
    [SerializeField] private ButtonBehaviour button4;
    [SerializeField] private ButtonBehaviour button5;
    [SerializeField] private ButtonBehaviour button6;
    [SerializeField] private ButtonBehaviour button7;
    [SerializeField] private ButtonBehaviour button8;
    [SerializeField] private ButtonBehaviour button9;
    [SerializeField] private ButtonBehaviour button0;
    [SerializeField] private ButtonBehaviour clearButton;

    private int[] _inputBuffer = new int[8] {-1, -1, -1, -1, -1, -1, -1, -1};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        RenderText();
    }

    private void CheckInput()
    {
        CheckAndApplyValue(button0, 0);
        CheckAndApplyValue(button1, 1);
        CheckAndApplyValue(button2, 2);
        CheckAndApplyValue(button3, 3);
        CheckAndApplyValue(button4, 4);
        CheckAndApplyValue(button5, 5);
        CheckAndApplyValue(button6, 6);
        CheckAndApplyValue(button7, 7);
        CheckAndApplyValue(button8, 8);
        CheckAndApplyValue(button9, 9);

        if (IsPressed(enterButton))
        {
            GameManager.Instance.SubmitInput(_inputBuffer);
            ClearInput();
            // TODO submit to game manager
        }

        if (IsPressed(clearButton))
        {
            ClearInput();
        }
    }

    public void ClearInput()
    {
        for (int i = 0; i < _inputBuffer.Length; i++)
        {
            _inputBuffer[i] = -1;
        }
    }

    private void CheckAndApplyValue(ButtonBehaviour button, int value)
    {
        if (IsPressed(button))
        {
            for (int i = 0; i < _inputBuffer.Length; i++)
            {
                if (_inputBuffer[i] == -1)
                {
                    _inputBuffer[i] = value;
                    break;
                }
            }
        }
    }
    
    private bool IsPressed(ButtonBehaviour button)
    {
        return button.GetValue() > 0f;
    }

    private void RenderText()
    {
        _inputText.text = "";
        for (int i = 0; i < _inputBuffer.Length; i++)
        {
            if (_inputBuffer[i] != -1)
            {
                _inputText.text += _inputBuffer[i].ToString();
            }
        }
    }
}
