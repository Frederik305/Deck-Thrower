using UnityEngine;

public class TrackingCard : MonoBehaviour
{
    private Rigidbody2D rb;
    public float homingAmount = 5f;
    public float detectionRadius = 10f;
    public float cardSpeed = 5f;
    public float rotationSpeed = 5f;
    
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
            // Calculer la direction vers l'ennemi
            Vector2 direction = (target.position - transform.position).normalized;

            // Calculer l'angle pour que l'objet pointe vers l'ennemi
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Interpoler entre l'angle actuel et l'angle cible pour une rotation plus douce
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle - 90f, Time.deltaTime * rotationSpeed);

            // Appliquer la rotation douce
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothAngle));

            // Appliquer la vitesse dans cette direction
            rb.linearVelocity = direction * cardSpeed;

            // Ajouter une force pour suivre l'ennemi
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
