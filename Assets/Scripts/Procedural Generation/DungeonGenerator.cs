using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static RoomData;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomData> roomPrefabs;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapCollisions;
    [SerializeField] private Tilemap doorsCollisions;
    [SerializeField] private Tilemap portalCollisions;
    [SerializeField] private List<TileBase> tileReferences;
    [SerializeField] private int maxRooms = 10;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;

    [Header("Rooms probabilities (0.5 = 50%)")]
    [SerializeField] private float chancesBossRoom = 0.0f;


    private HashSet<Vector2Int> occupiedTiles = new();
    private List<RoomInstance> placedRooms = new();
    private Queue<int> lastPickedRoomIds = new();
    private List<GameObject> enemyInstance = new();

    private void Start()
    {
        GenerateDungeon();
    }

    public void ReGenerateDungeon()
    {
        occupiedTiles.Clear();
        placedRooms.Clear();
        lastPickedRoomIds.Clear();

        foreach (GameObject enemy in enemyInstance)
        {
            Destroy(enemy);
        }

        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        if (roomPrefabs.Count == 0) return;

        // Filter only starting rooms
        List<RoomData> startRooms = roomPrefabs.FindAll(room => room.roomType == RoomType.StartingRoom);
        if (startRooms.Count == 0) return; // Ensure there is at least one starting room

        GameObject player = GameObject.FindWithTag("Player");

        player.transform.position = new Vector3(10, 5, 0);

        // Select a random start room
        RoomData startRoom = startRooms[Random.Range(0, startRooms.Count)];
        PlaceRoomWithoutSpawns(startRoom, Vector2Int.zero);

        // Try to add additional rooms using your current placement logic.
        for (int i = 1; i < maxRooms; i++)
        {
            TryPlaceNextRoom();
        }
        if (Random.value < chancesBossRoom)
        {
            // Attempt to place BossRoom if possible, otherwise place EndRoom
            if (!PlaceBossRoom())
            {
                // If BossRoom can't be placed, place EndRoom instead.
                PlaceEndRoom();
            }
        }
        else
        {
            PlaceEndRoom();
        }
    }

    /// <summary>
    /// Attempts to place a BossRoom by trying different flip variations until a door aligns with an exit of the last placed room.
    /// Returns true if the BossRoom was placed, false otherwise.
    /// </summary>
    private bool PlaceBossRoom()
    {
        List<RoomData> bossRooms = roomPrefabs.FindAll(room => room.roomType == RoomType.BossRoom);
        if (bossRooms.Count == 0) return false; // No boss rooms available, return false

        // Pick a random boss room.
        RoomData bossRoomOriginal = bossRooms[Random.Range(0, bossRooms.Count)];

        // Use the last placed room as the connection point.
        RoomInstance lastRoom = placedRooms[placedRooms.Count - 1];

        // Try a few flip variations. (Expand this list if you need more rotations.)
        bool[] flipOptions = new bool[] { false, true };
        foreach (bool rotate in flipOptions)
        {
            foreach (bool flipHorizontal in flipOptions)
            {
                foreach (bool flipVertical in flipOptions)
                {
                    // Rotate/flip the boss room variant.
                    RoomData bossRoomVariant = RotateTiles(bossRoomOriginal, rotate, flipHorizontal, flipVertical);

                    // For each exit on the last room, see if we can align a door.
                    foreach (var exit in lastRoom.roomData.exits)
                    {
                        // Get door groups for the boss room facing the opposite direction.
                        List<List<Vector2Int>> bossDoorGroups = GetExitGroups(bossRoomVariant, GetOppositeDirection(exit.direction));
                        foreach (var bossDoorGroup in bossDoorGroups)
                        {
                            Vector2Int bossDoorCenter = GetExitCenter(bossDoorGroup);
                            // Calculate the position where the boss room should be placed.
                            Vector2Int bossRoomPosition = lastRoom.gridPosition + GetDoorOffset(exit.direction, exit.position, bossDoorCenter);

                            if (!Overlaps(bossRoomVariant, bossRoomPosition))
                            {
                                PlaceRoomWithSpawns(bossRoomVariant, bossRoomPosition);

                                Vector2 roomCenter = ComputeRoomCenter(bossRoomVariant, bossRoomPosition);

                                GameObject enemyInstance = Instantiate(
                                    boss,
                                    new Vector3(roomCenter.x, roomCenter.y, 0),
                                    Quaternion.identity
                                );

                                return true; // Successfully placed the BossRoom
                            }
                        }
                    }
                }
            }
        }
        return false; // Couldn't place the BossRoom, return false
    }

    private Vector2 ComputeRoomCenter(RoomData room, Vector2Int gridPos)
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var tile in room.tiles)
        {
            Vector2Int pos = gridPos + tile.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
        }
        float centerX = (minX + maxX + 1) / 2f;
        float centerY = (minY + maxY + 1) / 2f;
        return new Vector2(centerX, centerY);
    }



    /// <summary>
    /// If there isn�t enough space for a BossRoom, attempts to place an EndRoom instead, aligning its door(s) with the last room.
    /// </summary>
    private void PlaceEndRoom()
    {
        List<RoomData> endRooms = roomPrefabs.FindAll(room => room.roomType == RoomType.EndRoom);
        if (endRooms.Count == 0) return;

        // Pick a random EndRoom.
        RoomData endRoomOriginal = endRooms[Random.Range(0, endRooms.Count)];

        // Use the last placed room as the connection point.
        RoomInstance lastRoom = placedRooms[placedRooms.Count - 1];

        // Try several flip variations.
        bool[] flipOptions = new bool[] { false, true };
        foreach (bool rotate in flipOptions)
        {
            foreach (bool flipHorizontal in flipOptions)
            {
                foreach (bool flipVertical in flipOptions)
                {
                    RoomData endRoomVariant = RotateTiles(endRoomOriginal, rotate, flipHorizontal, flipVertical);
                    foreach (var exit in lastRoom.roomData.exits)
                    {
                        List<List<Vector2Int>> endDoorGroups = GetExitGroups(endRoomVariant, GetOppositeDirection(exit.direction));
                        foreach (var endDoorGroup in endDoorGroups)
                        {
                            Vector2Int endDoorCenter = GetExitCenter(endDoorGroup);
                            Vector2Int endRoomPosition = lastRoom.gridPosition + GetDoorOffset(exit.direction, exit.position, endDoorCenter);

                            if (!Overlaps(endRoomVariant, endRoomPosition))
                            {
                                PlaceRoomWithoutSpawns(endRoomVariant, endRoomPosition);
                                return; // Exit after placing the EndRoom
                            }
                        }
                    }
                }
            }
        }
    }
    private void TryPlaceNextRoom()
    {
        foreach (var placedRoom in placedRooms)
        {
            foreach (var exit in placedRoom.roomData.exits)
            {
                // Pick a RegularRoom
                List<RoomData> regularRooms = roomPrefabs.FindAll(room => room.roomType == RoomType.RegularRoom);
                if (regularRooms.Count == 0) return;

                // Get a random index using your weighted method.
                int randomIndex = GetWeightedRandomIndex(regularRooms);
                RoomData newRoom = regularRooms[randomIndex];
                newRoom = RotateTiles(newRoom, false /*GetRandomFlip()*/, GetRandomFlip(), GetRandomFlip());

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
                        // Check if the exit is blocked by a wall tile.
                        Vector2Int exitWorldPos = placedRoom.gridPosition + exit.position;
                        Vector3Int worldPos = new Vector3Int(exitWorldPos.x, exitWorldPos.y, 0);

                        if (tilemapCollisions.GetTile(worldPos) != null)
                        {
                            // Try to unblock the exit with a variant of the room.
                            List<List<Vector2Int>> newRoomExitGroups = GetExitGroups(newRoom, GetOppositeDirection(exit.direction));
                            foreach (var newRoomExitGroup in newRoomExitGroups)
                            {
                                Vector2Int newRoomExitCenter = GetExitCenter(newRoomExitGroup);
                                Vector2Int newRoomExitPosition = placedRoom.gridPosition + GetDoorOffset(exit.direction, exit.position, newRoomExitCenter);

                                if (!Overlaps(newRoom, newRoomExitPosition))
                                {
                                    PlaceRoomWithSpawns(newRoom, newRoomExitPosition);
                                    // Only update history when placement is successful:
                                    lastPickedRoomIds.Enqueue(randomIndex);
                                    if (lastPickedRoomIds.Count > 3)
                                    {
                                        lastPickedRoomIds.Dequeue();
                                    }
                                    return; // Exit early after placing the new room
                                }
                            }
                        }
                        else
                        {
                            // Place the room normally.
                            PlaceRoomWithSpawns(newRoom, newRoomPosition);
                            // Only update history when placement is successful:
                            lastPickedRoomIds.Enqueue(randomIndex);
                            if (lastPickedRoomIds.Count > 3)
                            {
                                lastPickedRoomIds.Dequeue();
                            }
                            return;
                        }
                    }
                }
            }
        }
    }


    int GetWeightedRandomIndex(List<RoomData> rooms)
    {
        List<float> weights = new List<float>();

        // Calculate weights
        for (int i = 0; i < rooms.Count; i++)
        {
            int timesPicked = lastPickedRoomIds.Count(id => id == i);
            float weight = Mathf.Pow(0.25f, timesPicked); // Exponential decay factor
            weights.Add(weight);
        }

        // Select based on weights
        float totalWeight = weights.Sum();
        float randomPoint = Random.value * totalWeight;

        float currentSum = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            currentSum += weights[i];
            if (randomPoint <= currentSum)
                return i;
        }

        return 0; // Fallback (should never happen)
    }

    private bool GetRandomFlip()
    {
        return Random.value > 0.5f;
    }


    private RoomData RotateTiles(RoomData roomData, bool isRotated, bool flipX, bool flipY)
    {
        RoomData transformedRoomData = new RoomData
        {
            roomType = roomData.roomType,
            size = roomData.size,
            tiles = new List<RoomData.TileData>(),
            exits = new List<RoomData.ExitPoint>()
        };

        Vector2Int size = roomData.size;

        // Transform tile positions
        foreach (var tile in roomData.tiles)
        {
            Vector2Int transformedPos = tile.position;

            // Apply 90-degree clockwise rotation
            if (isRotated) transformedPos = new Vector2Int(transformedPos.y, size.x - 1 - transformedPos.x);

            // Apply flipping within bounds
            if (flipX) transformedPos.x = size.x - 1 - transformedPos.x;
            if (flipY) transformedPos.y = size.y - 1 - transformedPos.y;

            transformedRoomData.tiles.Add(new RoomData.TileData
            {
                position = transformedPos,
                tileName = tile.tileName
            });
        }

        // Transform exit positions and directions
        foreach (var exit in roomData.exits)
        {
            Vector2Int transformedExitPos = exit.position;
            RoomData.Direction newDirection = exit.direction;

            // Apply rotation
            if (isRotated)
            {
                transformedExitPos = new Vector2Int(transformedExitPos.y, size.x - 1 - transformedExitPos.x);
                newDirection = RotateDirectionClockwise(exit.direction);
            }

            // Apply flipping
            if (flipX)
            {
                transformedExitPos.x = size.x - 1 - transformedExitPos.x;
                newDirection = FlipDirectionX(newDirection);
            }
            if (flipY)
            {
                transformedExitPos.y = size.y - 1 - transformedExitPos.y;
                newDirection = FlipDirectionY(newDirection);
            }

            transformedRoomData.exits.Add(new RoomData.ExitPoint
            {
                position = transformedExitPos,
                direction = newDirection
            });
        }

        return transformedRoomData;
    }

    // Rotates direction 90 degrees clockwise
    private RoomData.Direction RotateDirectionClockwise(RoomData.Direction dir)
    {
        return dir switch
        {
            RoomData.Direction.North => RoomData.Direction.East,
            RoomData.Direction.East => RoomData.Direction.South,
            RoomData.Direction.South => RoomData.Direction.West,
            RoomData.Direction.West => RoomData.Direction.North,
            _ => dir
        };
    }

    // Mirrors direction across the X-axis
    private RoomData.Direction FlipDirectionX(RoomData.Direction dir)
    {
        return dir switch
        {
            RoomData.Direction.East => RoomData.Direction.West,
            RoomData.Direction.West => RoomData.Direction.East,
            _ => dir
        };
    }

    // Mirrors direction across the Y-axis
    private RoomData.Direction FlipDirectionY(RoomData.Direction dir)
    {
        return dir switch
        {
            RoomData.Direction.North => RoomData.Direction.South,
            RoomData.Direction.South => RoomData.Direction.North,
            _ => dir
        };
    }

    private void PlaceRoomWithSpawns(RoomData roomData, Vector2Int gridPos)
    {
        int enemiesSpawned = 0;

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
            if (tile.tileName.Contains("Wall"))
            {
                tilemapCollisions.SetTile(worldPos, tileBase);
            }
            if (tile.tileName.Contains("Door"))
            {
                doorsCollisions.SetTile(worldPos, tileBase);
            }
            if (tile.tileName.Contains("Portal"))
            {
                portalCollisions.SetTile(worldPos, tileBase);
            }
            else if (tileBase != null && !tile.tileName.Contains("Wall") && !tile.tileName.Contains("Door") && !tile.tileName.Contains("Portal"))
            {
                tilemap.SetTile(worldPos, tileBase);

                if (Random.value < 0.1f && enemiesSpawned < 3)
                {
                    Vector3 spawnPos = tilemap.GetCellCenterWorld(worldPos);
                    enemyInstance.Add(Instantiate(enemies[Random.Range(0, enemies.Count)], spawnPos, Quaternion.identity));
                    enemiesSpawned++;
                }
            }
        }
    }

    private void PlaceRoomWithoutSpawns(RoomData roomData, Vector2Int gridPos)
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
            if (tile.tileName.Contains("Wall"))
            {
                tilemapCollisions.SetTile(worldPos, tileBase);
            }
            if (tile.tileName.Contains("Door"))
            {
                doorsCollisions.SetTile(worldPos, tileBase);
            }
            if (tile.tileName.Contains("Portal"))
            {
                portalCollisions.SetTile(worldPos, tileBase);
            }
            else if (tileBase != null && !tile.tileName.Contains("Wall") && !tile.tileName.Contains("Door") && !tile.tileName.Contains("Portal"))
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
