using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] TileBase doorTile;
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 playerPosition = other.transform.position;

            Vector3Int tilePosition = tilemap.WorldToCell(playerPosition);

            // Get the tile at that position
            TileBase tile = tilemap.GetTile(tilePosition);

            if(tile == doorTile) 
            {
                Debug.Log("Player passed through a door at " + tilePosition);
            }
        }
    }
}
