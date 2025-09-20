using TMPro;
using UnityEngine;

public class GameTimeTestScene : SingletonMonoBehaviour<GameTimeTestScene>
{
    [SerializeField] private TextMeshProUGUI _secondsText;
    [SerializeField] private TextMeshProUGUI _currentTimeText;

    public void Start()
    {
        GameData.Instance.LoadGameData();
    }

    private void Update()
    {
        if (!GameTimeManager.Instance.IsInitialized) return;

        _secondsText.SetText($"{GameTimeManager.Instance.CurrentGameTime.TotalSeconds} seconds passed");
        _currentTimeText.SetText(TimeUtil.GameTimeToString(GameTimeManager.Instance.CurrentGameTime));
    }

    public void OnStartButtonClick()
    {
        if (GameTimeManager.Instance.IsInitialized)
        {
            GameTimeManager.Instance.Clear();
        }
        else
        {
            var startGameTime = new GameTime();
            startGameTime.SetTime(0);
            GameTimeManager.Instance.Initialize(startGameTime);
        }
    }
}