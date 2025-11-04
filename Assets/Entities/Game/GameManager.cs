using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _waveDisplayLocation;
    [SerializeField] private GameObject _waveDispalyPrefab;
    [SerializeField]private int _currentLevel = 0;
    [SerializeField] private int[] _levelWaveformCounts = { 1, 2, 4, 6, 8 };

    private List<WaveformController> _currentWaveforms = new List<WaveformController>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    private void ClearCurrentWaveforms()
    {
        _currentWaveforms.ForEach(waveform =>
        {
            Destroy(waveform.gameObject);
        });
        _currentWaveforms.Clear();
    }
}
