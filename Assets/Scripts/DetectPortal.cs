using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectPortal : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tilemap tilemapDoors;
    [SerializeField] private Tilemap tilemapPortal;
    [SerializeField] private DungeonGenerator dungeonGenerator;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player passed through a portal at ");
            tilemap.ClearAllTiles();
            tilemapWall.ClearAllTiles();
            tilemapDoors.ClearAllTiles();
            tilemapPortal.ClearAllTiles();

            dungeonGenerator.ReGenerateDungeon();
        }
    }
}
