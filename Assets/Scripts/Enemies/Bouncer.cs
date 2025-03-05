using UnityEngine;

// Classe restructurée par Logan avec l'héritage de Enemy
public class Bouncer : Enemy
{
    public float speed; // Vitesse de déplacement de l'ennemi
    public float torque; // Couple appliqué pour faire tourner l'ennemi
    private bool isMoving; // Indicateur si l'ennemi est en mouvement ou non

    // Start est appelé avant la première mise à jour
    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de la classe parente (Enemy)
        isActive = true; // L'ennemi est activé au début
        transform.Rotate(0, 0, Random.Range(0, 360)); // Applique une rotation aléatoire initiale à l'ennemi
        rb.linearVelocity = transform.up * speed; // Applique une vitesse initiale basée sur l'orientation de l'ennemi
        rb.AddTorque(torque); // Applique un couple pour faire tourner l'ennemi
    }

    // FixedUpdate est appelé à chaque frame fixe (pour la physique)
    private void FixedUpdate()
    {
        if (isActive)
        {   
            // Si l'ennemi est actif, on normalise la vélocité pour garantir une vitesse constante
            rb.linearVelocity = rb.linearVelocity.normalized * speed;

            // Si l'ennemi n'est pas en mouvement, applique une nouvelle rotation aléatoire
            // et réinitialise la vitesse et le couple
            if(!isMoving){
                transform.Rotate(0, 0, Random.Range(0, 360)); // Rotation aléatoire
                rb.linearVelocity = transform.up * speed; // Applique une nouvelle vitesse
                rb.AddTorque(torque); // Applique un nouveau couple
            }
            isMoving = true; // L'ennemi est maintenant en mouvement
        }
        else
        {
            isMoving = false; // Si l'ennemi n'est pas actif, il n'est plus en mouvement
        }

        // Gère l'activation ou la désactivation de l'ennemi selon la distance avec le joueur
        HandleActivation();
    }

    // Cette méthode est appelée lorsqu'une collision avec un autre objet se produit
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Joue un son de rebond lorsque l'ennemi entre en collision
        FindObjectOfType<AudioManager>().playSound("Bounce");
    }
}
