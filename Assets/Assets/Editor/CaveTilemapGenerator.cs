using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveTilemapGenerator : EditorWindow
{
    public TileBase groundTile;
    public TileBase grassTopTile;

    private const int Width = 95;
    private const int Height = 45;

    [MenuItem("Tools/Generate Cave Tilemap")]
    public static void ShowWindow()
    {
        GetWindow<CaveTilemapGenerator>("Cave Tilemap Generator");
    }

    private void OnGUI()
    {
        groundTile = (TileBase)EditorGUILayout.ObjectField("Ground Tile", groundTile, typeof(TileBase), false);
        grassTopTile = (TileBase)EditorGUILayout.ObjectField("Top Edge Tile", grassTopTile, typeof(TileBase), false);

        if (GUILayout.Button("Generate Tilemap"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        GameObject root = new GameObject("Generated_Cave_Level");

        GameObject gridObject = new GameObject("Grid");
        gridObject.transform.parent = root.transform;

        Grid grid = gridObject.AddComponent<Grid>();

        GameObject tilemapObject = new GameObject("Ground");
        tilemapObject.transform.parent = gridObject.transform;

        Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
        tilemapObject.AddComponent<TilemapRenderer>();
        tilemapObject.AddComponent<TilemapCollider2D>();

        bool[,] solid = new bool[Width, Height];

        FillAll(solid);

        // main cave rooms / tunnels
        CarveRect(solid, 5, 5, 15, 4);
        CarveRect(solid, 12, 9, 20, 5);
        CarveRect(solid, 25, 6, 30, 4);
        CarveRect(solid, 45, 8, 37, 5);

        CarveRect(solid, 7, 18, 17, 6);
        CarveRect(solid, 22, 15, 18, 7);
        CarveRect(solid, 37, 18, 30, 5);
        CarveRect(solid, 62, 15, 20, 7);

        CarveRect(solid, 5, 31, 25, 5);
        CarveRect(solid, 28, 28, 22, 6);
        CarveRect(solid, 50, 30, 25, 5);
        CarveRect(solid, 76, 28, 10, 8);

        // vertical shafts
        CarveRect(solid, 12, 2, 4, 12);
        CarveRect(solid, 33, 14, 4, 18);
        CarveRect(solid, 68, 20, 5, 14);
        CarveRect(solid, 82, 12, 5, 24);

        // small platforms / openings
        CarveRect(solid, 18, 22, 8, 3);
        CarveRect(solid, 40, 24, 8, 3);
        CarveRect(solid, 55, 25, 10, 3);
        CarveRect(solid, 72, 36, 10, 3);

        // bottom-right elevator pocket
        CarveRect(solid, 82, 37, 5, 5);

        // draw tiles
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!solid[x, y]) continue;

                Vector3Int pos = new Vector3Int(x, -y, 0);

                bool hasAirAbove = y > 0 && !solid[x, y - 1];

                if (hasAirAbove && grassTopTile != null)
                {
                    tilemap.SetTile(pos, grassTopTile);
                }
                else
                {
                    tilemap.SetTile(pos, groundTile);
                }
            }
        }

        tilemap.CompressBounds();
        Selection.activeGameObject = root;
    }

    private void FillAll(bool[,] solid)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                solid[x, y] = true;
            }
        }
    }

    private void CarveRect(bool[,] solid, int startX, int startY, int width, int height)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    solid[x, y] = false;
                }
            }
        }
    }
}