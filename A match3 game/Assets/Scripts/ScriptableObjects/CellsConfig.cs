using UnityEngine;

[CreateAssetMenu(fileName = "CellsConfig", menuName = "Cells/New CellsConfig")]
public class CellsConfig : ScriptableObject
{
    [SerializeField] private Sprite _glass1;
    [SerializeField] private Sprite _glass2;
    [SerializeField] private CellController _cellPrefab;
    public CellController CellPrefab => _cellPrefab;

    public Sprite GetCellSprite(int x, int y)
    {
        if (x % 2 != 0 && y % 2 != 0 || x % 2 == 0 && y % 2 == 0)
        {
            return _glass1;
        }
        else
        {
            return _glass2;
        }
    }


}
