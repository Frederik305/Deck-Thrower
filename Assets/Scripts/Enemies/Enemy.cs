using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    public int health = 10;    //The enemy's health

    protected GameManager gameManager;
    public Slider healthBar; // Référence au Slider (barre de vie)
    protected ScoreManager scoreManager;
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
        if (gameObject.tag == "Boss")
        {
            SceneManager.LoadScene(2);
        }
        Destroy(gameObject);
    }
}
