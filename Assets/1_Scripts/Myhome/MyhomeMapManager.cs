using Sirenix.OdinInspector;
using UnityEngine;

public class MyhomeMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    [SerializeField] private PlayerObject _player;
    [SerializeField] private int _sortingOrderOffset = 10;

    private void Start()
    {
        SetMapObjectSortingLayer();
    }

    private void Update()
    {
        _player.SetSortingLayer(_sortingOrderOffset);
    }

    [Button]
    private void SetMapObjectSortingLayer()
    {
        foreach (var mapObject in _mapObjects)
        {
            mapObject.SetSortingLayer(_sortingOrderOffset);
        }
    }
}