using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileDigging : MonoBehaviour
{
    private Grid grid;
    private List<Tilemap> tilemaps;

    [Header("Dig Settings")]
    [SerializeField] private float digDistance = 0.8f;

    private InputSystem inputActions;

    private void Awake()
    {
        inputActions = new InputSystem();
    }

    public void AssignTiles(Grid newGrid, List<Tilemap> newTilemaps)
    {
        grid = newGrid;
        tilemaps = newTilemaps;
        Debug.Log($"TileDigger switched to grid {grid.name}, tilemaps count: {tilemaps.Count}");
    }

    private void OnEnable()
    {
        inputActions.Player.Dig.performed += OnDig;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Dig.performed -= OnDig;
        inputActions.Player.Disable();
    }

    private void OnDig(InputAction.CallbackContext ctx)
    {
        if (grid == null || tilemaps == null) return;

        Vector2 move = inputActions.Player.DigDirection.ReadValue<Vector2>();
        Vector2 digDirection = SnapToCardinal(move);

        if (digDirection == Vector2.zero) return;

        Vector3 targetWorldPos = transform.position + (Vector3)(digDirection * digDistance);
        PerformDig(targetWorldPos, digDirection);
    }

    private void PerformDig(Vector3 worldPos, Vector2 direction)
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