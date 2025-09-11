using UnityEngine;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    public void Start()
    {
        GameData.Instance.LoadGameData();
        Debug.Log("LoadGameData");
        var a = GameData.Instance.DTKeyTestData;
        var a1 = GameData.Instance.DTMapData;
        var a2 = GameData.Instance.DTTestData;
    }
}