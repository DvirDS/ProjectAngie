using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDigging : MonoBehaviour
{
    private Grid grid;
    private List<Tilemap> tilemaps;

    [Header("Dig Settings")]
    [SerializeField] private float digDistance = 0.8f;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;

    private Vector3 pendingDigWorldPos;
    private bool hasPendingDig;

    public void AssignTiles(Grid newGrid, List<Tilemap> newTilemaps)
    {
        grid = newGrid;
        tilemaps = newTilemaps;
    }

    private void OnEnable()
    {
        if (input != null) input.OnDigPressed += OnDig;
    }

    private void OnDisable()
    {
        if (input != null) input.OnDigPressed -= OnDig;
    }

    private void OnDig()
    {
        if (grid == null || tilemaps == null) return;

        Vector2 digDirection = SnapToCardinal(input.DigDirection);
        if (digDirection == Vector2.zero) return;

        pendingDigWorldPos = transform.position + (Vector3)(digDirection * digDistance);
        hasPendingDig = true;
    }

    public void OnDigImpact()
    {
        if (!hasPendingDig) return;
        hasPendingDig = false;
        PerformDig(pendingDigWorldPos);
    }

    private void PerformDig(Vector3 worldPos)
    {
        Vector3Int centerCell = grid.WorldToCell(worldPos);

        DeleteAtCell(centerCell);

        Physics2D.SyncTransforms();
    }

    private void DeleteAtCell(Vector3Int cell)
    {
        foreach (var tilemap in tilemaps)
        {
            if (tilemap != null && tilemap.HasTile(cell))
            {
                tilemap.SetTile(cell, null);
            }
        }
    }

    private Vector2 SnapToCardinal(Vector2 raw)
    {
        if (raw.sqrMagnitude < 0.25f) return Vector2.zero;
        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            return raw.x > 0 ? Vector2.right : Vector2.left;
        return raw.y > 0 ? Vector2.up : Vector2.down;
    }
}
