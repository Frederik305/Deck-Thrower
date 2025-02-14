using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomData> roomPrefabs;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> tileReferences;
    [SerializeField] private int maxRooms = 10;

    private HashSet<Vector2Int> occupiedTiles = new();
    private List<RoomInstance> placedRooms = new();

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        if (roomPrefabs.Count == 0) return;

        // Start with the first room at (0,0)
        RoomData startRoom = roomPrefabs[0];
        PlaceRoom(startRoom, Vector2Int.zero);

        // Generate additional rooms
        for (int i = 1; i < maxRooms; i++)
        {
            TryPlaceNextRoom();
        }

        Debug.Log($"Dungeon Generated with {placedRooms.Count} rooms.");
    }

    private void TryPlaceNextRoom()
    {
        foreach (var placedRoom in placedRooms)
        {
            foreach (var exit in placedRoom.roomData.exits)
            {
                RoomData newRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                List<RoomData.ExitPoint> matchingExits = FindMatchingExits(newRoom, exit);

                if (matchingExits.Count == 0) continue;

                List<Vector2Int> exitPositions = GetExitPositions(placedRoom.roomData, exit.direction);
                List<Vector2Int> matchingPositions = GetExitPositions(newRoom, GetOppositeDirection(exit.direction));

                Vector2Int newRoomPosition = placedRoom.gridPosition + GetDoorOffset(exit.direction, exitPositions, matchingPositions);

                if (!Overlaps(newRoom, newRoomPosition))
                {
                    PlaceRoom(newRoom, newRoomPosition);
                    return;
                }
            }
        }
    }

    private List<Vector2Int> GetExitPositions(RoomData room, RoomData.Direction direction)
    {
        return room.exits.FindAll(exit => exit.direction == direction).ConvertAll(exit => exit.position);
    }




    private void PlaceRoom(RoomData roomData, Vector2Int gridPos)
    {
        // Store occupied tiles
        foreach (var tileData in roomData.tiles)
        {
            occupiedTiles.Add(gridPos + tileData.position);
        }

        placedRooms.Add(new RoomInstance(roomData, gridPos));

        // Render room
        foreach (var tileData in roomData.tiles)
        {
            TileBase tile = GetTileByName(tileData.tileName);
            if (tile != null)
            {
                Vector3Int worldPos = new Vector3Int(gridPos.x + tileData.position.x, gridPos.y + tileData.position.y, 0);
                tilemap.SetTile(worldPos, tile);
            }
        }
    }

    private bool Overlaps(RoomData room, Vector2Int pos)
    {
        foreach (var tile in room.tiles)
        {
            Vector2Int checkPos = pos + tile.position;

            if (!room.exits.Exists(exit => exit.position == tile.position) && occupiedTiles.Contains(checkPos))
                return true;
        }
        return false;
    }


    private List<RoomData.ExitPoint> FindMatchingExits(RoomData room, RoomData.ExitPoint targetExit)
    {
        RoomData.Direction oppositeDir = GetOppositeDirection(targetExit.direction);

        List<RoomData.ExitPoint> matchingExits = room.exits.FindAll(exit =>
            exit.direction == oppositeDir
        );

        return matchingExits;
    }


    private Vector2Int GetDoorOffset(RoomData.Direction dir, List<Vector2Int> exitPositions, List<Vector2Int> matchingExitPositions)
    {
        Vector2Int exitCenter = GetExitCenter(exitPositions);
        Vector2Int matchingCenter = GetExitCenter(matchingExitPositions);

        return exitCenter - matchingCenter + GetDirectionalOffset(dir);
    }

    private Vector2Int GetExitCenter(List<Vector2Int> exitPositions)
    {
        if (exitPositions.Count == 1) return exitPositions[0];

        int sumX = 0, sumY = 0;
        foreach (var pos in exitPositions)
        {
            sumX += pos.x;
            sumY += pos.y;
        }

        return new Vector2Int(sumX / exitPositions.Count, sumY / exitPositions.Count);
    }

    private Vector2Int GetDirectionalOffset(RoomData.Direction dir)
    {
        return dir switch
        {
            RoomData.Direction.North => new Vector2Int(0, 1),
            RoomData.Direction.South => new Vector2Int(0, -1),
            RoomData.Direction.East => new Vector2Int(1, 0),
            RoomData.Direction.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
    }



    private RoomData.Direction GetOppositeDirection(RoomData.Direction dir)
    {
        return dir switch
        {
            RoomData.Direction.North => RoomData.Direction.South,
            RoomData.Direction.South => RoomData.Direction.North,
            RoomData.Direction.East => RoomData.Direction.West,
            RoomData.Direction.West => RoomData.Direction.East,
            _ => dir
        };
    }

    private TileBase GetTileByName(string tileName)
    {
        return tileReferences.Find(tile => tile != null && tile.name == tileName);
    }

    private class RoomInstance
    {
        public RoomData roomData;
        public Vector2Int gridPosition;
        public RoomInstance(RoomData room, Vector2Int pos)
        {
            roomData = room;
            gridPosition = pos;
        }
    }
}
