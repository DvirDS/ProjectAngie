using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileDigging : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] public Grid grid;
    [SerializeField] public List<Tilemap> tilemaps;

    [Header("Dig Settings")]
    [Tooltip("כמה רחוק מהשחקן החפירה מתבצעת. נסה ערכים בין 0.5 ל-1.0")]
    [SerializeField] private float digDistance = 0.8f;

    [Tooltip("האם למחוק טייל בודד (false) או להרחיב מעט (true)")]
    [SerializeField] private bool useWideDig = false;

    private InputSystem inputActions;

    private void Awake()
    {
        inputActions = new InputSystem();
    }

    private void OnEnable()
    {
        Events.onUnloadAssignTiles += AssignTiles;
        inputActions.Player.Dig.performed += OnDig;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Dig.performed -= OnDig;
        Events.onUnloadAssignTiles -= AssignTiles;
        inputActions.Player.Disable();
    }

    private void OnDig(InputAction.CallbackContext ctx)
    {
        if (grid == null || tilemaps == null) return;

        Vector2 move = inputActions.Player.DigDirection.ReadValue<Vector2>();
        Vector2 digDirection = SnapToCardinal(move);

        if (digDirection == Vector2.zero) return;

        // חישוב המיקום המדויק מול אנג'י
        Vector3 targetWorldPos = transform.position + (Vector3)(digDirection * digDistance);
        PerformDig(targetWorldPos, digDirection);
    }

    private void PerformDig(Vector3 worldPos, Vector2 direction)
    {
        Vector3Int centerCell = grid.WorldToCell(worldPos);

        // מחיקת הטייל הספציפי בלבד
        DeleteAtCell(centerCell);

        // רק אם סימנת ב-Inspector, זה ימחק שכנים (כרגע כבוי כברירת מחדל)
        if (useWideDig)
        {
            if (direction == Vector2.left || direction == Vector2.right)
            {
                DeleteAtCell(centerCell + Vector3Int.up);
                DeleteAtCell(centerCell + Vector3Int.down);
            }
            else
            {
                DeleteAtCell(centerCell + Vector3Int.left);
                DeleteAtCell(centerCell + Vector3Int.right);
            }
        }

        // --- התיקון הקריטי למניעת היתקעות בלי להגדיל את החור ---
        // הסנכרון הזה מוודא שהפיזיקה מעודכנת לטייל שנמחק, גם אם הוא בודד.
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
}