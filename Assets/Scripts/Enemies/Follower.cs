using UnityEngine;

// Classe restructurée par Logan avec l'héritage de Enemy
public class Follower : Enemy
{
    public float homingAmount = 1f; // Détermine la force d'attraction de l'ennemi vers le joueur

    // Start est appelé avant la première mise à jour
    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de la classe parente (Enemy)
        isActive = true; // L'ennemi est activé au début
    }

    // FixedUpdate est appelé à chaque frame fixe (pour la physique)
    void FixedUpdate()
    {
        if (isActive)
        {
            // Calcul de la différence de position entre l'ennemi et le joueur
            Vector2 difference = player.position - transform.position;

            // Applique une force d'attraction à l'ennemi vers le joueur en fonction de la différence de position
            // La force est multipliée par homingAmount pour ajuster l'intensité de l'attraction
            rb.AddForce(difference * homingAmount * Time.deltaTime);
        }

        // Gère l'activation ou la désactivation de l'ennemi selon la distance avec le joueur
        HandleActivation();
    }

    // Cette méthode est appelée lorsqu'une collision avec un autre objet se produit
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Joue un son de rebond léger lorsque l'ennemi entre en collision
        FindObjectOfType<AudioManager>().playSound("Light Bounce");
    }
}
