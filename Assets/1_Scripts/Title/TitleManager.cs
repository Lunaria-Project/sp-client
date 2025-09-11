public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    public void Start()
    {
        GameData.Instance.LoadGameData();
    }
}