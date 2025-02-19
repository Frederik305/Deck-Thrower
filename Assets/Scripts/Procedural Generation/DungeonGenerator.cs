using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomData> roomPrefabs;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapCollisions;
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

        RoomData startRoom = roomPrefabs[0];
        PlaceRoom(startRoom, Vector2Int.zero);

        // Try to add additional rooms using your current placement logic.
        for (int i = 1; i < maxRooms; i++)
        {
            TryPlaceNextRoom();
        }

        // After placing the regular rooms, check each exit.
        // If an exit is blocked by a wall, try to fill it with a new room.
        FillBlockedExits();
    }

    private void TryPlaceNextRoom()
    {
        foreach (var placedRoom in placedRooms)
        {
            foreach (var exit in placedRoom.roomData.exits)
            {
                RoomData newRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                newRoom = RotateTiles(newRoom, GetRandomRotationDegree());

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

    /// <summary>
    /// Iterates over each placed room and its exits. If an exit is blocked by a wall tile,
    /// this method attempts to place another room that has a matching door on the opposite side.
    /// </summary>
    private void FillBlockedExits()
    {
        List<RoomInstance> currentRooms = new(placedRooms);

        foreach (var placedRoom in currentRooms)
        {
            foreach (var exit in placedRoom.roomData.exits)
            {
                Vector2Int exitWorldPos = placedRoom.gridPosition + exit.position;
                Vector3Int worldPos = new Vector3Int(exitWorldPos.x, exitWorldPos.y, 0);

                // Check if the exit is blocked by a wall tile
                if (tilemapCollisions.GetTile(worldPos) != null)
                {
                    // Select a new room to try to connect here.
                    RoomData newRoom = roomPrefabs[Random.Range(0, roomPrefabs.Count)];

                    // Get door groups for the new room with a door facing back toward the current room.
                    List<List<Vector2Int>> newRoomGroups = GetExitGroups(newRoom, GetOppositeDirection(exit.direction));
                    foreach (var newRoomGroup in newRoomGroups)
                    {
                        Vector2Int newRoomCenter = GetExitCenter(newRoomGroup);
                        // Calculate the position for the new room so that the door centers align.
                        Vector2Int newRoomPosition = placedRoom.gridPosition + GetDoorOffset(exit.direction, exit.position, newRoomCenter);

                        if (!Overlaps(newRoom, newRoomPosition))
                        {
                            PlaceRoom(newRoom, newRoomPosition);
                            break;
                        }
                    }
                }
            }
        }
    }

    private int GetRandomRotationDegree()
    {
        int[] validDegrees = { 0, 90, 180, 270 };

        int randomIndex = UnityEngine.Random.Range(0, validDegrees.Length);

        return validDegrees[randomIndex];
    }


    private RoomData RotateTiles(RoomData roomData, int degrees)
    {
        if (degrees != 180)
            return roomData;

        int numRotations = degrees / 90;

        // Create a new RoomData to hold the rotated data.
        RoomData rotatedRoomData = new RoomData
        {
            tiles = new List<RoomData.TileData>(),
            exits = new List<RoomData.ExitPoint>()
        };

        // Rotate tiles and store them in the new RoomData
        foreach (var tile in roomData.tiles)
        {
            Vector2Int rotatedTilePos = tile.position;
            for (int i = 0; i < numRotations; i++)
            {
                rotatedTilePos = new Vector2Int(-rotatedTilePos.y, rotatedTilePos.x);
            }

            rotatedRoomData.tiles.Add(new RoomData.TileData
            {
                position = rotatedTilePos,
                tileName = tile.tileName
            });
        }

        // Rotate exits and store them in the new RoomData
        foreach (var exit in roomData.exits)
        {
            RoomData.Direction rotatedExitDirection = (RoomData.Direction)(((int)exit.direction + numRotations) % 4);
            rotatedRoomData.exits.Add(new RoomData.ExitPoint
            {
                direction = rotatedExitDirection,
                position = exit.position
            });
        }

        return rotatedRoomData;  // Return the newly created rotated RoomData
    }


    private void RotateExits(RoomData roomData, int numRotations)
    {
        for (int i = 0; i < roomData.exits.Count; i++)
        {
            roomData.exits[i].direction = (RoomData.Direction)(((int)roomData.exits[i].direction + numRotations) % 4);
        }
    }

    private void PlaceRoom(RoomData roomData, Vector2Int gridPos)
    {
        // Mark all tiles as occupied.
        foreach (var tile in roomData.tiles)
        {
            occupiedTiles.Add(gridPos + tile.position);
        }

        placedRooms.Add(new RoomInstance(roomData, gridPos));

        // Render room and collision tiles.
        foreach (var tile in roomData.tiles)
        {
            Vector3Int worldPos = new Vector3Int(gridPos.x + tile.position.x, gridPos.y + tile.position.y, 0);
            TileBase tileBase = GetTileByName(tile.tileName);
            if (tile.tileName.Contains("wall"))
            {
                tilemapCollisions.SetTile(worldPos, tileBase);
            }
            else if (tileBase != null)
            {
                tilemap.SetTile(worldPos, tileBase);
            }
        }
    }

    private bool IsExitBlocked(RoomData room, Vector2Int pos)
    {
        foreach (var exit in room.exits)
        {
            // Compute absolute world position of exit
            Vector2Int exitPos = pos + exit.position;
            Vector3Int worldPos = new Vector3Int(exitPos.x, exitPos.y, 0);

            // Check if exit is blocked by a wall
            if (tilemapCollisions.GetTile(worldPos) != null)
            {
                return true; // Exit is directly blocked
            }

            // Check if there's a wall in front of the exit
            Vector2Int frontPos = exitPos + GetDirectionalOffset(exit.direction);
            Vector3Int frontWorldPos = new Vector3Int(frontPos.x, frontPos.y, 0);
            if (tilemapCollisions.GetTile(frontWorldPos) != null)
            {
                return true; // A wall is directly in front of the exit
            }
        }
        return false;
    }


    private bool Overlaps(RoomData room, Vector2Int pos)
    {
        // If any door would be blocked, we treat this as an overlap.
        if (IsExitBlocked(room, pos))
        {
            return true;
        }

        // Check for overlap with already occupied tiles.
        foreach (var tile in room.tiles)
        {
            Vector2Int checkPos = pos + tile.position;
            if (room.exits.Exists(exit => exit.position == tile.position))
                continue;
            if (occupiedTiles.Contains(checkPos))
                return true;
        }
        return false;
    }

    private Vector2Int GetDoorOffset(RoomData.Direction dir, Vector2Int placedCenter, Vector2Int matchingCenter)
    {
        return placedCenter - matchingCenter + GetDirectionalOffset(dir);
    }

    private List<List<Vector2Int>> GetExitGroups(RoomData room, RoomData.Direction direction)
    {
        List<Vector2Int> positions = room.exits
            .FindAll(exit => exit.direction == direction)
            .ConvertAll(exit => exit.position);

        // Sort positions based on door orientation.
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
                if (direction == RoomData.Direction.North || direction == RoomData.Direction.South)
                    isContiguous = (pos.x == lastPos.x + 1);
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
            _ => Vector2Int.zero,
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
            _ => dir,
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
            this.roomData = room;
            this.gridPosition = pos;
        }
    }
}
