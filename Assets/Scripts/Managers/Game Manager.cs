using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Keeps track of what wave the player is currently in
    int currentWave;

    //lootScreen is the ui screen that holds the loot cards for the player to choose
    private GameObject lootScreen;

    [SerializeField] private PauseMenu pauseMenu;
    //cardTypes is an array of each card type (taken from Shoot)
    private GameObject[] cardTypes;

    private GameObject player;

    //List of all enemy types
    public GameObject[] enemies;

    private int numEnemies;

    //Keeps track of whether or not the round just ended
    bool roundEnd;

    private GameObject gameOverScreen;
    private ScoreManager scoreManager;

    private int numKills;
    private int numScores;

    public TextMeshProUGUI roundDisplay;
    public TextMeshProUGUI killDisplay;
    public TextMeshProUGUI scoreDisplay;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        scoreManager = GetComponent<ScoreManager>();
        lootScreen = GameObject.FindGameObjectWithTag("Loot Screen");

        player = GameObject.FindGameObjectWithTag("Player");

        cardTypes = player.GetComponent<Shoot>().cards;

        gameOverScreen = GameObject.FindGameObjectWithTag("Game Over");
        gameOverScreen.gameObject.SetActive(false);

        currentWave = 1;

        numKills = 0;

        numScores = 0;

        //Mettre à jour l'affichage du score au démarrage
        UpdateScoreDisplay();

    }
    
private int nextThreshold = 3; // Prochain palier de score

//LS
private void Update()
{
    // Si le joueur appuie sur Échap, terminer la partie
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        
        pauseMenu.Pause();
    }

    // Vérifier si le score a dépassé le seuil actuel
    if (scoreManager.GetScore() >= nextThreshold)
    {
        lootScreen.GetComponent<LootScreen>().activate(cardTypes);
        

        // Augmenter le seuil pour la prochaine activation
        nextThreshold += 4;
    }
}

    public void UpdateEnemyCount()
    {
        numEnemies--;

        numKills++;

        AddScore(1);

    }

    public void GameOver ()
    {
        //Show game over screen
        gameOverScreen.gameObject.SetActive(true);

        //Show the number of rounds reached and the number of kills
        roundDisplay.text = (currentWave-1).ToString();
        killDisplay.text = numKills.ToString();
        Time.timeScale = 0f;
        UpdateScoreDisplay();
    }

    public void BackToMenu()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        SceneManager.LoadScene(3);
        Time.timeScale = 1f;
    }

    public void AddScore(int points)
    {
        numScores += points;
        scoreManager.AddScore(points);
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        scoreDisplay.text = numScores.ToString();
    }

}
