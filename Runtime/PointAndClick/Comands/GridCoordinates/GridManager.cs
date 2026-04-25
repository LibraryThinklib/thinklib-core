using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Grid/GridManager")]
public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [Header("Configuração do Grid")]
    [Tooltip("A posição do mundo (X, Y) da sua célula (0, 0) no canto superior esquerdo.")]
    public Vector2 gridOrigin;

    [Tooltip("O tamanho de cada célula do grid (em unidades do Unity).")]
    public float cellSize = 1.0f;

    private const string MechanicName = "PointAndClick/GridCoordinates/GridManager";

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; }
        instance = this;

        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(GridManager),
            new Dictionary<string, object>
            {
                { "cellSize", cellSize },
                { "gridOriginX", gridOrigin.x },
                { "gridOriginY", gridOrigin.y }
            });
    }

    public Vector3 GetWorldPosition(int row, int col)
    {
        float x = gridOrigin.x + (col * cellSize);
        float y = gridOrigin.y - (row * cellSize);
        return new Vector3(x, y, 0);
    }
}
