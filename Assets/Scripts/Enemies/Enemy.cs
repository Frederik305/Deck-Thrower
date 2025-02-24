using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    //The enemy's health
    public int health = 10;

    protected GameManager gameManager;
    public Slider healthBar; // Référence au Slider (barre de vie)
    protected ScoreManager scoreManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        scoreManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreManager>();
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
        Destroy(gameObject);
    }
}
