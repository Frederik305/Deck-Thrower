using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CustomEditor(typeof(RoomData))]
public class RoomDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomData roomData = (RoomData)target;
        if (GUILayout.Button("Find Doors & Set Room Data"))
        {
            UpdateRoomData(roomData);
        }
    }

    private void UpdateRoomData(RoomData roomData)
    {
        Tilemap tilemap = roomData.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("No Tilemap found in this room!");
            return;
        }

        // 1. Calculate Room Size
        BoundsInt bounds = tilemap.cellBounds;
        roomData.size = new Vector2Int(bounds.size.x, bounds.size.y);

        // 2. Find Doors
        List<RoomData.ExitPoint> exits = new List<RoomData.ExitPoint>();
        List<RoomData.TileData> tileDataList = new List<RoomData.TileData>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePos);

                if (tile != null)
                {
                    // Save tile data
                    tileDataList.Add(new RoomData.TileData
                    {
                        position = new Vector2Int(x - bounds.xMin, y - bounds.yMin),
                        tileName = tile.name
                    });

                    // Identify doors
                    if (tile.name.Contains("Door"))
                    {
                        RoomData.ExitPoint exit = new RoomData.ExitPoint
                        {
                            position = new Vector2Int(x - bounds.xMin, y - bounds.yMin),
                            direction = DetermineDirection(tilemap, x, y)
                        };
                        exits.Add(exit);
                    }
                }
            }
        }

        // 3. Apply to RoomData
        roomData.exits = exits;
        roomData.tiles = tileDataList;
        EditorUtility.SetDirty(roomData);
        Debug.Log($"Updated {roomData.gameObject.name}: Size {roomData.size}, {tileDataList.Count} tiles, {exits.Count} doors found.");
    }

    private RoomData.Direction DetermineDirection(Tilemap tilemap, int x, int y)
    {
        if (!tilemap.HasTile(new Vector3Int(x, y + 1, 0))) return RoomData.Direction.North;
        if (!tilemap.HasTile(new Vector3Int(x, y - 1, 0))) return RoomData.Direction.South;
        if (!tilemap.HasTile(new Vector3Int(x - 1, y, 0))) return RoomData.Direction.West;
        if (!tilemap.HasTile(new Vector3Int(x + 1, y, 0))) return RoomData.Direction.East;

        return RoomData.Direction.North; // Default fallback
    }
}
