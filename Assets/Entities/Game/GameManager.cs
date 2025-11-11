using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform _waveDisplayLocation;
    [SerializeField] private GameObject _waveDispalyPrefab;
    [SerializeField]private int _currentLevel = 0;
    [SerializeField] private int[] _levelWaveformCounts = { 1, 2, 4, 6, 8 };

    [SerializeField] private TestRadio _testRadio;

    private List<WaveformController> _currentWaveforms = new List<WaveformController>();

    private List<int> _solution = new List<int>();
    private Queue<int> _solutionQueue = new Queue<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    }

    public void SetupNextWaveformSet()
    {
        _testRadio.ResetSolutionState();
        ClearCurrentWaveforms();
        int counts = _levelWaveformCounts[_currentLevel];
        _currentLevel++;

        if (_currentLevel >= _levelWaveformCounts.Length)
        {
            _currentLevel = _levelWaveformCounts.Length - 1; // stay at max level
            Debug.Log("Max level reached");
        }

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
