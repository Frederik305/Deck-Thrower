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

    public TextMeshProUGUI roundDisplay;
    public TextMeshProUGUI killDisplay;

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

        SpawnEnemies();

        //endWave();
    }
    
private int nextThreshold = 3; // Prochain palier de score

//LS
private void Update()
{
    // Si le joueur appuie sur Échap, terminer la partie
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        player.SetActive(false);
        GameOver();
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

        //If there are no more enemies, end the wave
        if (numEnemies <= 0)
        {
            SpawnEnemies();
        }
    }

    public void GameOver ()
    {
        //Show game over screen
        gameOverScreen.gameObject.SetActive(true);

        //Show the number of rounds reached and the number of kills
        roundDisplay.text = (currentWave-1).ToString();
        killDisplay.text = numKills.ToString();
        Time.timeScale = 0f;
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

        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    /*private void endWave () {
        //lootScreen.GetComponent<LootScreen>().activate(cardTypes);

        player.GetComponent<shoot>().setEnableShooting(false);

        //This prevents the endWave function form running multiple times
        roundEnd = true;
    }

    //Once loot picked, get rid of loot screen and spawn in new enemies
    public void startWave(GameObject selectedCard)
    {
        //Card selected, so play the card selected sound
        FindObjectOfType<AudioManager>().playSound("Card Select");

        //Get rid of the loot screen
        lootScreen.transform.GetChild(0).gameObject.SetActive(false);

        //Enable shooting again
        player.GetComponent<shoot>().setEnableShooting(true);

        //Adds the card the player chose to their inventory
        //player.GetComponent<shoot>().addCard(selectedCard.GetComponent<LootOption>().getTypeIndex());

        spawnEnemies();

        roundEnd = false;
        
        currentWave++;

        //Make the player invincible for the very start of the round (prevents having an enemy spawn on you and hurting you)
        player.GetComponent<playerMovement>().resetInvincibilityCounter();
        //Also, give them one extra health
        player.GetComponent<playerMovement>().raiseHealth();
    }*/

    public void SpawnEnemies ()
    {
        Transform spawner = GameObject.FindGameObjectWithTag("Spawn").transform;

        ArrayList spawnPoints = new ArrayList();

        for (int x = 0; x < spawner.childCount; x++)
        {
            //Stores every spawn point in our array list
            spawnPoints.Add(spawner.GetChild(x));
        }

        numEnemies = 5;

        for (int x = 0; x < numEnemies; x++)
        {
            //If we are out of spawn points, then stop
            if (spawnPoints.Count <= 0)
            {
                numEnemies = x;
                break;
            }

            //Choose a random spawn point
            Transform spawnPoint = (Transform)spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            //Spawn a random enemy type there
            Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Length)], spawnPoint);

            //Remove spawn point from array list so we can't use it again
            spawnPoints.Remove(spawnPoint);
        }
    }
}
