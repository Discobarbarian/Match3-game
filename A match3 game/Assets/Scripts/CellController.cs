using UnityEngine;

public class CellController : MonoBehaviour
{
    [SerializeField] private CellsConfig _cellsConfig;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public bool IsEmpty { get; private set;}
    public bool IsTaken { get; set;}
    public bool IsValid => !IsEmpty && !IsTaken; 

    public void Init(int x, int y, bool empty) 
    {
        IsEmpty = empty;
        if(!empty)
        {
            _spriteRenderer.sprite = _cellsConfig.GetCellSprite(x, y);
        }
    }
}
