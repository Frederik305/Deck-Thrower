using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //Starts the main game scene
    public void startGame ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        SceneManager.LoadScene(1);
    }

    //Opens the Rules panel
    /*public void openRules ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        Panel.GameObject.SetActive(true);   //TODO Mettre le nom du panel
    }*/

    //Closes the Rules panel
    /*public void closeRules ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        Panel.GameObject.SetActive(false);  //TODO Mettre le nom du panel
    }*/

    //Opens the Options panel
    /*public void openOptions ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        Panel.GameObject.SetActive(true);   //TODO Mettre le nom du panel
    }*/

    //Closes the Options panel
    /*public void closeOptions ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        Panel.GameObject.SetActive(false);  //TODO Mettre le nom du panel
    }*/

    //Closes the game
    public void quit ()
    {
        FindObjectOfType<AudioManager>().playSound("Button Click");

        Application.Quit();
    }
}
