using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileDigging : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] public List<Tilemap> tilemaps;
    [SerializeField] private float offsetMultiplier = 1.1f; 

    InputSystem inputActions;

    private void Awake()
    {
        inputActions = new InputSystem();
    }

    private void OnEnable()
    {
        Events.onUnloadAssignTiles += assignTiles;
        inputActions.Player.Dig.performed += OnDig;
        Events.onUnloadAssignTiles += AssignTiles;
    }

    private void OnDisable()
    {
        inputActions.Player.Dig.performed -= OnDig;
        inputActions.Player.Disable();
        Events.onUnloadAssignTiles -= AssignTiles;
    }

    private void OnDig(InputAction.CallbackContext ctx)
    {
        if (grid == null) return;

        Vector2 move = inputActions.Player.DigDirection.ReadValue<Vector2>();
        Vector2 digDirection = SnapToCardinal(move);
        if (digDirection == Vector2.zero)
            return;

        Vector3 tilePosition = transform.position + (Vector3)OffsetForDirection(digDirection);
        RemoveTileAt(tilePosition);
    }

    private void RemoveTileAt(Vector3 tilePosition)
    {
        if (grid == null || tilemaps == null || tilemaps.Count == 0) return;

        Vector3Int cell = grid.WorldToCell(tilePosition);

        if (tilemaps.Any(tilemap => tilemap != null && tilemap.HasTile(cell)))
        {
            foreach (var tilemap in tilemaps)
                tilemap.SetTile(cell, null);
        }
    }

    private void assignTiles(Grid grid, List<Tilemap> tilemaps)
    {
        RoomTilemapProvider tilemapProvider = FindFirstObjectByType<RoomTilemapProvider>();
        if (tilemapProvider != null)
        {
            this.grid = grid;
            this.tilemaps = tilemaps;
        }
        else Debug.LogWarning($"{name}: No RoomTilemapProvider found in scene.");
        
        inputActions.Player.Enable();
        inputActions.Player.Dig.performed += OnDig;
    }

    private void AssignTiles(Grid newGrid, List<Tilemap> newTilemaps)
    {
        grid = newGrid;
        tilemaps = newTilemaps;
    }

    private Vector2 SnapToCardinal(Vector2 raw)
    {
        if (raw.sqrMagnitude < 0.25f) return Vector2.zero;

        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            return raw.x > 0 ? Vector2.right : Vector2.left;

        return raw.y > 0 ? Vector2.up : Vector2.down;
    }

    private Vector2 OffsetForDirection(Vector2 dir)
    {
        Vector3 cellSize = grid.cellSize;
        float directionX = cellSize.x * offsetMultiplier;
        float directionY = cellSize.y * offsetMultiplier;

        if (dir == Vector2.left) return new Vector2(-directionX, 0f);
        if (dir == Vector2.right) return new Vector2(directionX, 0f);
        if (dir == Vector2.up) return new Vector2(0f, directionY);
        return new Vector2(0f, -directionY); 
    }


}
