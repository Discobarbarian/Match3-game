using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ChipsConfig", menuName ="Chips/New ChipsConfig")]
public class ChipsConfig : ScriptableObject
{
    [SerializeField] private ChipController _chipPrefab;
    [SerializeField] private List<Sprite> _chipsSprites;

    public ChipController ChipPrefab => _chipPrefab;

    public Sprite GetChipSprite()
    {
        int index = Random.Range(0, _chipsSprites.Count - 1);
        return _chipsSprites[index];

    }
}
