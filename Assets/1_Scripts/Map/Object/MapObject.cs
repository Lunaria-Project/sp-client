using System;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] protected Transform _transform;
    [SerializeField] protected SpriteRenderer _sprite;

    public void SetSortingLayer(int offset)
    {
        if (string.Equals(_sprite.sortingLayerName, NameContainer.SortingLayer.MapObject, StringComparison.Ordinal))
        {
            _sprite.sortingOrder = offset - Mathf.RoundToInt(_transform.localPosition.y * 10);
        }
    }
}