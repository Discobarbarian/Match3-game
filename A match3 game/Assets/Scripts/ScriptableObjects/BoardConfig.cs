using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="fieldConfig", menuName ="Fields/New fieldConfig")]
public class BoardConfig : ScriptableObject
{
    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeY;
    [SerializeField] [Range(0f, 0.5f)] private float _emptynessChance;
    [SerializeField] private float _spawnDelay;

    public int sizeX => _sizeX;
    public int sizeY => _sizeY;
    public float SpawnDelay => _spawnDelay;
 
    public bool IsCellEmpty()
    {
        float chance = Random.Range(0f, 1f);
        return chance <= _emptynessChance;
    }
}
[CustomEditor(typeof(BoardConfig))]
public class BoardConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Cells"))
        {
            FindObjectOfType<BoardContoller>().GenerateCells();
        }
    }
}
