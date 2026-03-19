using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTilemapProvider : MonoBehaviour
{
    public Grid grid;
    public List<Tilemap> tilemaps;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        tilemaps = new List<Tilemap>(GetComponentsInChildren<Tilemap>(includeInactive: false));
        Events.onUnloadAssignTiles?.Invoke(grid, tilemaps);
    }
}
