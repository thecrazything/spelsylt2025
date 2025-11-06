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

    private List<WaveformController> _currentWaveforms = new List<WaveformController>();

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
        ClearCurrentWaveforms();
        int counts = _levelWaveformCounts[_currentLevel];
        _currentLevel++;

        if (_currentLevel >= _levelWaveformCounts.Length)
        {
            _currentLevel = _levelWaveformCounts.Length - 1; // stay at max level
            Debug.Log("Max level reached");
        }

        List<WaveFormModel> waveforms = WaveFormModel.GetRandom(counts);

        waveforms.ForEach(waveform =>
        {
            GameObject waveformObj = Instantiate(_waveDispalyPrefab, _waveDisplayLocation);
            WaveformController controller = waveformObj.GetComponent<WaveformController>();
            controller.SetWaveform(waveform);
            _currentWaveforms.Add(controller);
        });
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
}
