using UnityEngine;

public class Boss : Enemy
{
    // Paramètres de tir
    public float shootTime = 1f;
    private float timer;
    public GameObject bullet;
    public float bulletForce = 15f;

    // Paramètres de distance pour l'activation et la désactivation

    //public bool isActive = true;

    // Méthode Start() qui initialise l'ennemi, appelée une fois au démarrage
    protected override void Start()
    {
        base.Start(); // Appel à la méthode Start() de la classe parente Enemy
        isActive = true;
        timer = 0;    // Initialisation du timer
    }

    // Mise à jour appelée à chaque frame
    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime; // Ajoute le temps écoulé au timer
            if (timer >= shootTime)  // Vérifie si le temps de tir est écoulé
            {
                Shoot();  // Appelle la méthode Shoot()
                timer = 0; // Réinitialise le timer
            }
        }

    }

    // Méthode pour tirer un projectile vers le joueur
    public void Shoot()
    {
        Vector2 difference = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
        difference = difference.normalized; // Normalise la direction

        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity); // Crée une balle
        newBullet.GetComponent<Rigidbody2D>().AddForce(difference * bulletForce); // Applique une force au Rigidbody2D
    }
}
