using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform _waveDisplayLocation;
    [SerializeField] private GameObject _waveDispalyPrefab;
    [SerializeField]private int _currentLevel = 0;
    [SerializeField] private int[] _levelWaveformCounts = { 1, 2, 2, 4, 8 };
    [SerializeField] private int[] _levelCountdownTimes = { -1, -1, 60, 120, 240 };

    [SerializeField] private TestRadio _testRadio;
    [SerializeField] private CountdownClock _countdownClock;
    [SerializeField] private Light _warningLight;

    [SerializeField] private float _coutdownValue = -1f;
    [SerializeField] private bool _isCountdownActive = false;
    [SerializeField] private AudioSource _gameOverAudioSource;
    [SerializeField] private AudioSource _countdownAudioSource;

    private List<WaveformController> _currentWaveforms = new List<WaveformController>();

    private List<int> _solution = new List<int>();
    private Queue<int> _solutionQueue = new Queue<int>();

    [SerializeField] private BlackoutBehaviour _blackoutBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _blackoutBehaviour.FadeOut();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        SetupNextWaveformSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCountdownActive)
        {
            if (!_countdownAudioSource.isPlaying)
            {
                _countdownAudioSource.Play();
            }
            _warningLight.enabled = true;
            _coutdownValue -= Time.deltaTime;
            if (_coutdownValue <= 0f)
            {
                _coutdownValue = 0f;
                _isCountdownActive = false;
                _countdownClock.isOn = false;
                Debug.Log("Countdown ended - player failed to complete in time");
                _testRadio.ForceOff = true;
                _testRadio.TurnOff();
                _gameOverAudioSource.Play();
                _blackoutBehaviour.FadeIn();
                // TODO handle end of countdown (e.g. fail state)
            }
            if (_coutdownValue <= 60f)
            {
                _countdownClock.isOn = true;
                _countdownClock.text.text = Mathf.CeilToInt(_coutdownValue).ToString();
            }
            else
            {
                _countdownClock.isOn = false;
            }
        }
        else
        {
            _warningLight.enabled = false;
            _countdownClock.isOn = false;
            if (_countdownAudioSource.isPlaying)
            {
                _countdownAudioSource.Stop();
            }
        }
    }

    public void SetupNextWaveformSet()
    {
        _countdownAudioSource.Stop();
        _warningLight.enabled = false;
        _countdownClock.isOn = false;
        _testRadio.ResetSolutionState();
        ClearCurrentWaveforms();
        int counts = _levelWaveformCounts[_currentLevel];

        List<WaveFormModel> waveforms = WaveFormModel.GetRandom(counts);
        _solution.Clear();
        waveforms.ForEach(waveform =>
        {
            GameObject waveformObj = Instantiate(_waveDispalyPrefab, _waveDisplayLocation);
            WaveformController controller = waveformObj.GetComponent<WaveformController>();
            controller.SetWaveform(waveform);
            _currentWaveforms.Add(controller);
            _solution.Add(UnityEngine.Random.Range(0, 10));
        });
        _solutionQueue = new Queue<int>(_solution);

        int countdownTime = _levelCountdownTimes[_currentLevel];
        if (countdownTime > 0)
        {
            _isCountdownActive = true;
            _coutdownValue = countdownTime;
        }
        else
        {
            _isCountdownActive = false;
        }

        _currentLevel++;

        if (_currentLevel >= _levelWaveformCounts.Length)
        {
            _currentLevel = _levelWaveformCounts.Length - 1; // stay at max level
            Debug.Log("Max level reached");
        }
    }
    
    public WaveFormModel GetClosestMatch(WaveFormModel other)
    {
        WaveFormModel closest = null;
        bool[] closetsMatching = null;
        _currentWaveforms.ForEach(waveform =>
        {
            var model = waveform.Model;
            var match = model.Matches(other);
            if (closest == null)
            {
                closest = model;
                closetsMatching = match;
            }
            else
            {
                int currentMatchCount = 0;
                int closestMatchCount = 0;
                foreach (var m in match)
                {
                    if (m) currentMatchCount++;
                }
                foreach (var m in closetsMatching)
                {
                    if (m) closestMatchCount++;
                }
                if (currentMatchCount > closestMatchCount)
                {
                    closest = model;
                    closetsMatching = match;
                }
            }
        });
        return closest;
    }

    private void ClearCurrentWaveforms()
    {
        _currentWaveforms.ForEach(waveform =>
        {
            Destroy(waveform.gameObject);
        });
        _currentWaveforms.Clear();
    }

    internal void TurnOffAllWaveforms()
    {
        _currentWaveforms.ForEach(waveform =>
        {
            waveform.gameObject.SetActive(false);
        });
    }

    internal void TurnOnAllWaveforms()
    {
        _currentWaveforms.ForEach(waveform =>
        {
            waveform.gameObject.SetActive(true);
        });
    }

    internal int ClearWaveform(WaveFormModel targetModel)
    {
        WaveformController controller = _currentWaveforms.Find(waveform => waveform.Model == targetModel);
        Destroy(controller.gameObject);
        _currentWaveforms.Remove(controller);
        return _solutionQueue.Dequeue();
    }

    internal void SubmitInput(int[] inputBuffer)
    {
        for (int i = 0; i < _solution.Count; i++)
        {
            if (_solution[i] != inputBuffer[i])
            {
                Debug.Log("Incorrect code submitted");
                return;
            }
        }
        Debug.Log("Correct code submitted, advancing to next level");
        SetupNextWaveformSet();
    }
}
