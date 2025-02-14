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

                // Group door positions for the placed room for this exit's direction.
                List<List<Vector2Int>> placedGroups = GetExitGroups(placedRoom.roomData, exit.direction);
                // Find the group that contains the current exit's position.
                List<Vector2Int> placedGroup = placedGroups.Find(g => g.Contains(exit.position));
                if (placedGroup == null)
                    continue;
                Vector2Int placedCenter = GetExitCenter(placedGroup);

                // For the new room, get groups of door positions in the opposite direction.
                List<List<Vector2Int>> newRoomGroups = GetExitGroups(newRoom, GetOppositeDirection(exit.direction));
                foreach (var newRoomGroup in newRoomGroups)
                {
                    Vector2Int newRoomCenter = GetExitCenter(newRoomGroup);
                    Vector2Int newRoomPosition = placedRoom.gridPosition + GetDoorOffset(exit.direction, placedCenter, newRoomCenter);

                    if (!Overlaps(newRoom, newRoomPosition))
                    {
                        PlaceRoom(newRoom, newRoomPosition);
                        return;
                    }
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


    private Vector2Int GetDoorOffset(RoomData.Direction dir, Vector2Int placedCenter, Vector2Int matchingCenter)
    {
        return placedCenter - matchingCenter + GetDirectionalOffset(dir);
    }

    private List<List<Vector2Int>> GetExitGroups(RoomData room, RoomData.Direction direction)
    {
        // Get all door positions for the given direction
        List<Vector2Int> positions = room.exits
            .FindAll(exit => exit.direction == direction)
            .ConvertAll(exit => exit.position);

        // Sort positions based on direction
        if (direction == RoomData.Direction.North || direction == RoomData.Direction.South)
            positions.Sort((a, b) => a.x.CompareTo(b.x));
        else
            positions.Sort((a, b) => a.y.CompareTo(b.y));

        List<List<Vector2Int>> groups = new List<List<Vector2Int>>();
        List<Vector2Int> currentGroup = new List<Vector2Int>();

        foreach (var pos in positions)
        {
            if (currentGroup.Count == 0)
            {
                currentGroup.Add(pos);
            }
            else
            {
                Vector2Int lastPos = currentGroup[currentGroup.Count - 1];
                bool isContiguous = false;

                // For North/South, group by adjacent x values
                if (direction == RoomData.Direction.North || direction == RoomData.Direction.South)
                    isContiguous = (pos.x == lastPos.x + 1);
                // For East/West, group by adjacent y values
                else
                    isContiguous = (pos.y == lastPos.y + 1);

                if (isContiguous)
                {
                    currentGroup.Add(pos);
                }
                else
                {
                    groups.Add(new List<Vector2Int>(currentGroup));
                    currentGroup.Clear();
                    currentGroup.Add(pos);
                }
            }
        }
        if (currentGroup.Count > 0)
            groups.Add(currentGroup);

        return groups;
    }


    private Vector2Int GetExitCenter(List<Vector2Int> group)
    {
        int sumX = 0, sumY = 0;
        foreach (var pos in group)
        {
            sumX += pos.x;
            sumY += pos.y;
        }
        return new Vector2Int(sumX / group.Count, sumY / group.Count);
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
