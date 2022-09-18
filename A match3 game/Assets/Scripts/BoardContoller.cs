using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardContoller : MonoBehaviour
{
    [SerializeField] private BoardConfig _boardConfig;
    [SerializeField] private CellsConfig _cellsConfig;
    [SerializeField] private ChipsConfig _chipsConfig;
    [SerializeField] private NormalizeCamera _camera;
    private Dictionary<(int x, int y), ChipController> _chips = new Dictionary<(int x, int y), ChipController>();
    private Dictionary<(int x, int y), CellController> _cells = new Dictionary<(int x, int y), CellController>();
    private List<(int x, int y)> _spawnPoints = new List<(int x, int y)>();
    private Vector2 _offset => _boardPosition - new Vector2(_boardConfig.sizeX, _boardConfig.sizeY) / 2;
    private Vector2 _boardPosition => new Vector2(transform.position.x, transform.position.y);


    private void Awake()
    {
        GenerateCells();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGeneratingChips();
        }
    }

    private void StartGeneratingChips()
    {
        StopAllCoroutines();
        ClearChips();
        ClearCellFlags();
        StartCoroutine(GenerateChips());
        StartCoroutine(CheckFallingChips());
    }

    private void ClearCellFlags()
    {
        foreach (var cell in _cells)
        {
            cell.Value.IsTaken = false;
        }
    }

    private void ClearChips()
    {
        foreach (KeyValuePair<(int x, int y), ChipController> chip in _chips)
        {
            Destroy(chip.Value.gameObject);
        }
        _chips.Clear();
    }

    private void ClearCells()
    {
        foreach (KeyValuePair<(int x, int y), CellController> cell in _cells)
        {
            Destroy(cell.Value.gameObject);
        }
        _cells.Clear();
        _spawnPoints.Clear();
    }

    private Vector2 GetPositionCoordinate(int x, int y)
    {
        return _offset + new Vector2(x, y);
    }

    private Vector2 GetPositionCoordinate((int x, int y) coordinate)
    {
        return GetPositionCoordinate(coordinate.x, coordinate.y);
    }

    public void GenerateCells()
    {
        ClearChips();
        ClearCells();
        _camera.ZoomCamera();

        for (int x = 0; x < _boardConfig.sizeX; x++)
        {
            for (int y = 0; y < _boardConfig.sizeY; y++)
            {
                bool isEmpty = _boardConfig.IsCellEmpty();
                Vector2 position = GetPositionCoordinate(x, y);
                CellController cell = Instantiate(_cellsConfig.CellPrefab, position, Quaternion.identity, transform);
                cell.Init(x, y, isEmpty);
                _cells.Add((x, y), cell);
                if (y == _boardConfig.sizeY - 1)
                {
                    _spawnPoints.Add((x, y));
                }
            }
        }
    }

    private IEnumerator GenerateChips()
    {
        while (CanSpawn())
        {
            foreach ((int x, int y) spawnPoint in _spawnPoints)
            {
                if (!_cells[spawnPoint].IsValid) continue;
                Vector2 position = GetPositionCoordinate(spawnPoint);
                ChipController chip = Instantiate(_chipsConfig.ChipPrefab, position, Quaternion.identity, transform);
                chip.Init(this);
                chip.SetCoordinates(spawnPoint);
                _chips.Add(spawnPoint, chip);
                _cells[spawnPoint].IsTaken = true;
            }
            yield return new WaitForSeconds(_boardConfig.SpawnDelay);
        }
    }

    private bool CanFall(out Dictionary<(int x, int y), ChipController> chips)
    {
        chips = new Dictionary<(int x, int y), ChipController>();
        foreach (KeyValuePair<(int x, int y), ChipController> chip in _chips)
        {
            if (chip.Key.y > 0)
            {
                bool checkLeft = chip.Key.x > 0 && _cells[(chip.Key.x - 1, chip.Key.y)].IsEmpty
                                 && _cells[(chip.Key.x - 1, chip.Key.y - 1)].IsValid;
                bool checkRight = chip.Key.x < _boardConfig.sizeX - 1 && _cells[(chip.Key.x + 1, chip.Key.y)].IsEmpty
                                 && _cells[(chip.Key.x + 1, chip.Key.y - 1)].IsValid;
                if (!_cells[(chip.Key.x, chip.Key.y - 1)].IsTaken || checkLeft || checkRight)
                {
                    chips.Add(chip.Key, chip.Value);
                }
            }
        }
        return chips.Count > 0;
    }

    private IEnumerator CheckFallingChips()
    {
        Dictionary<(int x, int y), ChipController> chips = new Dictionary<(int x, int y), ChipController>();
        while (true)
        {
            if (CanFall(out chips)) ChipsFalling(chips);
            yield return null;
        }
    }

    private bool CanSpawn()
    {
        foreach ((int x, int y) spawnPoint in _spawnPoints)
        {
            if (!_cells[spawnPoint].IsTaken)
            {
                return true;
            }

        }
        return false;
    }

    private (int x, int y) GetWaypoint((int x, int y) coordinate)
    {
        for (int y = coordinate.y - 1; y >= 0; y--)
        {
            if (!_cells[(coordinate.x, y)].IsValid)
            {
                return (coordinate.x, y + 1);
            }
        }
        return (coordinate.x, 0);
    }

    private void ChipsFalling(Dictionary<(int x, int y), ChipController> chips)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (KeyValuePair<(int x, int y), ChipController> chip in chips)
        {
            List<Vector3> wayPoints = new List<Vector3>();
            (int x, int y) coordinates = GetWaypoint(chip.Key);

            if (coordinates.x > 0 && _cells[(coordinates.x - 1, coordinates.y)].IsEmpty)
            {
                if (coordinates.y > 0 && _cells[(coordinates.x - 1, coordinates.y - 1)].IsValid)
                {
                    wayPoints.Add(GetPositionCoordinate(coordinates));
                    wayPoints.Add(GetPositionCoordinate(coordinates.x - 1, coordinates.y - 1));
                    coordinates = GetWaypoint((coordinates.x - 1, coordinates.y - 1));
                }
            }
            else if (coordinates.x < _boardConfig.sizeX - 1 && _cells[(coordinates.x + 1, coordinates.y)].IsEmpty)
            {
                if (coordinates.y > 0 && _cells[(coordinates.x + 1, coordinates.y - 1)].IsValid)
                {
                    wayPoints.Add(GetPositionCoordinate(coordinates));
                    wayPoints.Add(GetPositionCoordinate(coordinates.x + 1, coordinates.y - 1));
                    coordinates = GetWaypoint((coordinates.x + 1, coordinates.y - 1));
                }
            }

            if (_chips.ContainsKey(coordinates)) continue;
            wayPoints.Add(GetPositionCoordinate(coordinates));
            _cells[coordinates].IsTaken = true;
            _cells[chip.Key].IsTaken = false;
            chip.Value.SetCoordinates(coordinates);
            _chips.Remove(chip.Key);
            _chips.Add(coordinates, chip.Value);
            chip.Value.transform.DOPath(wayPoints.ToArray(), 1, PathType.Linear);
        }
    }

    private List<ChipController> FindMatchingChips((int x, int y) CentralChipPosition)
    {
        List<ChipController> matchingChips = new List<ChipController>();
        for (int x = CentralChipPosition.x - 1; x <= CentralChipPosition.x + 1; x++)
        {
            for (int y = CentralChipPosition.y + 1; y >= CentralChipPosition.y - 1; y--)
            {
                if (x < 0 || x >= _boardConfig.sizeX) continue;
                if (y < 0 || y >= _boardConfig.sizeY) continue;
                if (!_cells[(x, y)].IsTaken) continue;
                if (_chips[(x, y)].SpriteRenderer.sprite == _chips[CentralChipPosition].SpriteRenderer.sprite)
                {
                    matchingChips.Add(_chips[(x, y)]);
                }
            }
        }
        return matchingChips;
    }

    public void DestroyChips((int x, int y) chipPosition)
    {
        List<ChipController> chipsToDestroy = FindMatchingChips(chipPosition);
        foreach (ChipController chip in chipsToDestroy)
        {
            (int x, int y) chipCoordinate = chip.Coordinates;
            _cells[chipCoordinate].IsTaken = false;
            _chips.Remove(chipCoordinate);
            Destroy(chip.gameObject);
        }
    }
}
