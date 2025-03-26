using UnityEngine;

public class EnemyBarrel : MonoBehaviour
{
    public float explosionRadius = 3f;
    public int explosionDamage = 1;
    public GameObject explosionEffect; // Assign an explosion animation in Unity

    public void Explode()
    {
        // Instantiate explosion effect (if assigned)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all objects in explosion radius
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in hitObjects)
        {
            if (col.CompareTag("Player"))
            {
                // Correctly reference playerMovement instead of Player
                playerMovement player = col.GetComponent<playerMovement>();
                if (player != null)
                {
                    player.health -= explosionDamage; // Reduce player health
                    player.healthScript.lowerHealth(player.health); // Update UI
                    player.resetInvincibilityCounter(); // Reset invincibility
                }
            }
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                Debug.Log("OUCHHHH Baril");
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }

        // Destroy the barrel after explosion
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}


