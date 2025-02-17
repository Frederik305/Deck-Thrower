using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{

    public GameObject[] cards;
    //public GameObject baseCards; // Cartes de base
    
    private List<GameObject> availableCards= new List<GameObject>();
    
    public int magazineSize = 5; // Nombre de cartes dans le chargeur
    private List<GameObject> magazine = new List<GameObject>();

    public GameObject icon;
    public float iconSelectedSize = 1.1f;
    public float iconWidth = 40f;

    
    private bool enableShooting = true;

    public float reloadTime = 2f; // Temps de rechargement en secondes
    private bool isReloading = false;

    void Start()
    {
        availableCards.Add(cards[5]); // Commence avec seulement les cartes de base
        FillMagazine(); // Remplit le chargeur immédiatement au début
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && enableShooting)
        {
            throwRandomCard();
        }
    }

    public void setEnableShooting(bool newState)
    {
        enableShooting = newState;
    }

    void throwRandomCard()
    {
        if (magazine.Count == 0)
        {
            StartCoroutine(ReloadMagazine());
            return;
        }

        // Prendre la première carte du chargeur et la lancer
        GameObject selectedCard = magazine[0];
        magazine.RemoveAt(0);

        GameObject newCard = Instantiate(selectedCard);
        newCard.transform.position = transform.position;
        newCard.transform.rotation = transform.rotation;

        FindFirstObjectByType<AudioManager>().playSound("Card Throw");
    }

    IEnumerator ReloadMagazine()
    {
        if (isReloading) yield break; // Empêche un rechargement multiple

        isReloading = true;
        enableShooting = false;

        Debug.Log("Rechargement en cours...");
        //FindObjectOfType<AudioManager>().playSound("Reload");

        yield return new WaitForSeconds(reloadTime);

        magazine.Clear();
        for (int i = 0; i < magazineSize; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            magazine.Add(availableCards[randomIndex]);
        }

        Debug.Log("Magasin rechargé !");
        isReloading = false;
        enableShooting = true;
    }
    
    void FillMagazine()
    {
        magazine.Clear();
        for (int i = 0; i < magazineSize; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            magazine.Add(availableCards[randomIndex]);
        }
        Debug.Log("Chargeur initial rempli !");
    }

    public void UnlockCard(GameObject newCard)
    {
        if (!availableCards.Contains(newCard))
        {
            availableCards.Add(newCard);
            Debug.Log("Nouvelle carte ajoutée au chargeur : " + newCard.name);
             FillMagazine(); // Remplit le chargeur immédiatement au début
        }
    }

}
