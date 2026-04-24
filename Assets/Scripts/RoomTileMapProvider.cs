using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTilemapProvider : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private List<Tilemap> tilemaps;

    private void Awake()
    {
        if (grid == null)
        {
            Debug.LogError("Grid reference is missing on RoomTilemapProvider.");
        }
        if (tilemaps == null || tilemaps.Count == 0)
        {
            Debug.LogError("Tilemaps list is empty on RoomTilemapProvider.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by {other.name}");

        if (!other.CompareTag("Player")) return;

        TileDigging digger = other.GetComponent<TileDigging>();
        if (digger == null) return;

        digger.AssignTiles(grid, tilemaps);
        Debug.Log($"Player entered room with grid {grid.name}");
    }
}