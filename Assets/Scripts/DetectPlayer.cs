using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TEST");
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 playerPosition = other.transform.position;

            Vector3Int tilePosition = tilemap.WorldToCell(playerPosition);

            // Get the tile at that position
            TileBase tile = tilemap.GetTile(tilePosition);

            Debug.Log("Player passed through a door at " + tilePosition);
        }
    }
}
