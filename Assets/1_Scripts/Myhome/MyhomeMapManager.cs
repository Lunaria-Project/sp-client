using Sirenix.OdinInspector;
using UnityEngine;

public class MyhomeMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;

    private void Start()
    {
        SetMapObjectSortingLayer();
    }

    [Button]
    private void SetMapObjectSortingLayer()
    {
        foreach (var mapObject in _mapObjects)
        {
            mapObject.SetSortingLayer(10);
        }
    }
}