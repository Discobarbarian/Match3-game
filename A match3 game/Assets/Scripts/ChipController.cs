using UnityEngine;

public class ChipController : MonoBehaviour
{
    [SerializeField] private ChipsConfig _chipsConfig;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private BoardContoller _boardContoller;
    private (int x, int y) _coordinates;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public (int x, int y) Coordinates => _coordinates;

    public void Init(BoardContoller boardController)
    {
        _boardContoller = boardController;
        _spriteRenderer.sprite = _chipsConfig.GetChipSprite();
    }

    private void OnMouseDown()
    {
        _boardContoller.DestroyChips(_coordinates);
    }

    public void SetCoordinates((int x, int y) coordinates)
    {
        _coordinates = coordinates;
    }
}
