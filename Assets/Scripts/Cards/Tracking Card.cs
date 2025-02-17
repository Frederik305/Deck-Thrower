using UnityEngine;

public class TrackingCard : MonoBehaviour
{
    private Rigidbody2D rb;
    public float homingAmount = 5f;
    public float detectionRadius = 10f;
    public float cardSpeed = 5f;
    
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindClosestEnemy();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * cardSpeed;
            rb.AddForce(direction * homingAmount * Time.deltaTime);
        }
        else
        {
            FindClosestEnemy();
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = detectionRadius;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }
}
