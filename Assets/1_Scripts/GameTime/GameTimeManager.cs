using System;
using UnityEngine;

public class GameTimeManager : SingletonMonoBehaviour<GameTimeManager>
{
    public event Action OnIntervalChanged;

    public GameTime CurrentGameTime => _currentGameTime;
    public bool IsInitialized => _isInitialized;
    
    private GameTime _currentGameTime;
    private bool _isPaused;
    private bool _isInitialized;
    private double _currentDaySeconds;
    private int _currentIntervalIndex = -1;
    
    private void Update()
    {
        if (!_isInitialized || _isPaused) return;
        if (GameSetting.Instance.SecondsPerGameHour <= 0) return;

        // 현실 Δt → 게임 초 환산: (1시간=3600초) * (Δt / 현실_초당_게임1시간)
        _currentDaySeconds += Time.deltaTime * (TimeUtil.MinutesPerHour * TimeUtil.SecondsPerMinute) / GameSetting.Instance.SecondsPerGameHour;
        
        var currentDaySecondsToInt = Mathf.FloorToInt((float)_currentDaySeconds);
        var tenMinuteIndex = TimeUtil.GetTenMinuteIntervalIndex(currentDaySecondsToInt);
        _currentGameTime.SetTime(currentDaySecondsToInt);
        if (_currentIntervalIndex == tenMinuteIndex) return;
        
        _currentIntervalIndex = tenMinuteIndex;
        OnIntervalChanged?.Invoke();
        
        //TODO(지선): 24시간이 넘을 경우는 이후에 작업하기
    }

    public void Clear()
    {
        _isInitialized = false;
        _currentDaySeconds = 0;
        _currentIntervalIndex = -1;
    }
    
    public void Initialize(GameTime startGameTime)
    {
        var hours = TimeUtil.SecondsToHours(startGameTime.TotalSeconds);
        if (hours >= TimeUtil.HoursPerDay)
        {
            LogManager.LogError("Hours per day is greater than TimeUtil. HoursPerDay");
            var totalSeconds = startGameTime.TotalSeconds - TimeUtil.SecondsPerDay;
            startGameTime.SetTime(totalSeconds);
            Initialize(startGameTime);
            return;
        }
        
        _currentGameTime = startGameTime;
        _currentDaySeconds = _currentGameTime.TotalSeconds;
        _currentIntervalIndex = TimeUtil.GetTenMinuteIntervalIndex(_currentGameTime.TotalSeconds);
        
        _isInitialized = true;
        _isPaused = false;
    }

    public void Pause()
    {
        if (!_isInitialized) return;
        _isPaused = true;
    }

    public void Resume()
    {
        if (!_isInitialized) return;
        _isPaused = false;
    }
}
