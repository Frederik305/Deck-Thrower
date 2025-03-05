using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Classe restructurée par Logan
public class Enemy : MonoBehaviour
{
    public int health = 10; // Points de vie de l'ennemi
    private Transform transform; // Référence au transform de l'ennemi (pour sa position)
    protected GameManager gameManager; // Référence au GameManager pour gérer le jeu
    public Slider healthBar; // Barre de vie de l'ennemi (UI)
    protected ScoreManager scoreManager; // Référence au ScoreManager pour gérer le score
    protected Transform player; // Référence au joueur
    protected Rigidbody2D rb; // Référence au Rigidbody2D pour gérer la physique de l'ennemi

    public float deactivationDistance = 20f; // Distance à laquelle l'ennemi sera désactivé
    public float reactivationDistance = 18f; // Distance à laquelle l'ennemi sera réactivé
    public bool isActive = true; // Si l'ennemi est actif ou non

    // Start est appelé avant la première mise à jour
    protected virtual void Start()
    {
        // Récupération des composants nécessaires pour la gestion du jeu
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        scoreManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        transform = gameObject.GetComponent<Transform>();

        // Initialisation de la barre de vie si elle est assignée
        if (healthBar != null)
        {
            healthBar.maxValue = health; // Valeur maximale de la barre de vie
            healthBar.value = health; // Valeur initiale de la barre de vie
        }
    }

    // Méthode de gestion de la distance pour activer ou désactiver l'ennemi
    protected void HandleActivation()
    {
        // Vérifie si le joueur est toujours dans la scène
        if (player != null)
        {
            // Calcule la distance entre l'ennemi et le joueur
            float distance = Vector2.Distance(transform.position, player.position);

            // Si l'ennemi est trop loin, il est désactivé
            if (distance > deactivationDistance)
            {
                isActive = false;
                rb.linearVelocity = Vector2.zero; // Arrête le mouvement de l'ennemi
                rb.angularVelocity = 0f; // Arrête la rotation de l'ennemi
            }
            // Si l'ennemi est assez proche, il est réactivé
            else if (distance < reactivationDistance)
            {
                isActive = true;
            }
        }
    }

    // Méthode pour infliger des dégâts à l'ennemi
    public void TakeDamage(int damage)
    {
        health -= damage; // Réduit la santé de l'ennemi

        // Met à jour la barre de vie si elle est présente
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        // Si l'ennemi n'a plus de vie, il meurt
        if (health <= 0)
        {
            Die();
        }
    }

    // Méthode appelée lorsque l'ennemi meurt
    protected virtual void Die()
    {
        // Met à jour le nombre d'ennemis dans le GameManager
        gameManager.UpdateEnemyCount();
        // Ajoute des points au score
        scoreManager.AddScore(1);

        // Si l'ennemi est un boss, change de scène
        if (gameObject.tag == "Boss")
        {
            SceneManager.LoadScene(2); // Charge la scène 2 (niveau suivant)
        }

        // Détruit l'ennemi
        Destroy(gameObject);
    }
}
