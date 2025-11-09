using UnityEngine;

public class NpcObject : MovableObject
{
    [SerializeField] private int _npcDataId;

    public int NpcDataId => _npcDataId;
}