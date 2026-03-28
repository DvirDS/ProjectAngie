using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTilemapProvider : MonoBehaviour
{
    private Grid grid;
    private List<Tilemap> tilemaps;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        tilemaps = new List<Tilemap>(GetComponentsInChildren<Tilemap>(includeInactive: false));
    }

    private void Start()
    {
        Events.onUnloadAssignTiles?.Invoke(grid, tilemaps);
    }
}
