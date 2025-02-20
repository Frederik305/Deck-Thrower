using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomType roomType;
    public Vector2Int size;
    public List<ExitPoint> exits = new();
    public List<TileData> tiles = new();
    public List<Vector2Int> spawnPoints = new();

    [System.Serializable]
    public class ExitPoint
    {
        public Vector2Int position;
        public Direction direction;
    }

    [System.Serializable]
    public class TileData
    {
        public Vector2Int position;
        public string tileName;
    }

    public enum Direction { North, South, East, West }
    public enum RoomType
    {
        RegularRoom,
        StartingRoom,
        EndRoom,
        BossRoom,
        TraderRoom,
        SecretRoom,
    }
}
