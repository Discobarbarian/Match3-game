using UnityEngine;

public class NormalizeCamera : MonoBehaviour
{
    [SerializeField] private BoardConfig _boardConfig;
    [SerializeField] private Camera _camera;

    public void ZoomCamera()
    {
        _camera.orthographicSize = Mathf.Max(_boardConfig.sizeY, _boardConfig.sizeX) / 2 + 3;
    }
}
