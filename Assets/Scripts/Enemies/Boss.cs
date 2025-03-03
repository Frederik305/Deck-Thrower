using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boss : MonoBehaviour
{
    //The enemy's health
    public int health = 50;

    protected GameManager gameManager;
    public Slider healthBar; // Référence au Slider (barre de vie)
    protected ScoreManager scoreManager;
    //public float deactivationDistance = 20f; // Distance maximale avant désactivation
    //public float reactivationDistance = 18f; // Distance minimale avant réactivation
    private Transform player;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        scoreManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        
        
        if (healthBar != null)
        {
            healthBar.maxValue = health;  // Définir la vie maximale
            healthBar.value = health;     // Définir la vie actuelle
        }
    }

    private void Update()
    {
        if (player != null)
        {
            /*float distance = Vector2.Distance(transform.position, player.position);
            if (distance > deactivationDistance)
            {
                rb.linearVelocity = Vector2.zero; // Arrêter l'ennemi
                rb.angularVelocity = 0f;   // Arrêter la rotation
            }
            else if (distance < reactivationDistance && rb.linearVelocity == Vector2.zero)
            {
                rb.linearVelocity = transform.up * 2f; // Redonner une vitesse de base à l'ennemi
            }*/
        }
    }

    public void TakeDamage (int damage)
    {
        health -= damage;
        // Mise à jour de la barre de vie
        if (healthBar != null)
        {
            healthBar.value = health; // Mettre à jour la barre de vie
        }
        //When out of health, die and let the Game Manager know
        if (health <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        gameManager.UpdateEnemyCount();
        scoreManager.AddScore(1);
        Destroy(gameObject);
    }
}
