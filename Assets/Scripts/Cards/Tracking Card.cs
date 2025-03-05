using UnityEngine;
//LS
/// <summary>
/// Cette classe gère le comportement d'une carte qui suit un ennemi dans un rayon donné.
/// Elle applique un suivi en douceur de la position de l'ennemi avec une rotation et une vitesse ajustable.
/// </summary>
public class TrackingCard : MonoBehaviour
{
    // Composant Rigidbody2D pour appliquer la physique
    private Rigidbody2D rb;

    // Le facteur qui détermine l'intensité du suivi
    public float homingAmount = 5f;

    // Le rayon de détection pour trouver l'ennemi le plus proche
    public float detectionRadius = 10f;

    // La vitesse de déplacement de la carte
    public float cardSpeed = 5f;

    // La vitesse de rotation de la carte
    public float rotationSpeed = 5f;

    // La cible que la carte suit (l'ennemi le plus proche)
    private Transform target;

    //LS
    /// <summary>
    /// Méthode appelée au début pour initialiser la carte et trouver l'ennemi le plus proche.
    /// </summary>
    void Start()
    {
        // Récupère le composant Rigidbody2D de la carte pour appliquer les forces
        rb = GetComponent<Rigidbody2D>();

        // Trouve l'ennemi le plus proche au démarrage
        FindClosestEnemy();
    }

    //LS
    /// <summary>
    /// Méthode appelée à chaque frame physique (FixedUpdate) pour mettre à jour la position
    /// et la rotation de la carte en fonction de la position de l'ennemi.
    /// </summary>
    void FixedUpdate()
    {
        // Si un ennemi a été trouvé
        if (target != null)
        {
            // Calculer la direction vers l'ennemi
            Vector2 direction = (target.position - transform.position).normalized;

            // Calculer l'angle pour que la carte pointe vers l'ennemi
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Interpoler entre l'angle actuel et l'angle cible pour une rotation plus douce
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle - 90f, Time.deltaTime * rotationSpeed);

            // Appliquer la rotation douce
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothAngle));

            // Appliquer la vitesse dans cette direction avec un lissage de la vitesse
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * cardSpeed, Time.deltaTime * homingAmount);

            // Ajouter une force pour aider au suivi
            rb.AddForce(direction * homingAmount * Time.deltaTime);
        }
        else
        {
            // Si aucun ennemi n'est trouvé, rechercher un nouvel ennemi
            FindClosestEnemy();
        }
    }
    //LS
    /// <summary>
    /// Méthode qui cherche l'ennemi le plus proche dans un rayon donné et met à jour la cible.
    /// </summary>
    void FindClosestEnemy()
    {
        // Trouver tous les objets ennemis dans la scène
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        // Définir la distance minimale initiale pour la détection
        float closestDistance = detectionRadius;
        Transform closestEnemy = null;

        // Vérifier chaque ennemi pour trouver celui qui est le plus proche
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        // Mettre à jour la cible avec l'ennemi le plus proche trouvé
        target = closestEnemy;
    }
}
