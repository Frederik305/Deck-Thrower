using System.Collections;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab; // Assign the Barrel Prefab in the Inspector
    public int numberOfBarrels = 5; // How many barrels to spawn
    public float spawnRadius = 5f; // How far from the center to spawn

    private void Start()
    {
        StartCoroutine(SpawnBarrels());
    }

    private IEnumerator SpawnBarrels()
    {
        for (int i = 0; i < numberOfBarrels; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Instantiate(barrelPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(1f); // Optional: Small delay between spawns
        }
    }
}