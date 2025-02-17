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
    public Slider reloadBar; // Référence à la barre de progression UI

    void Start()
    {
        availableCards.Add(cards[0]); // Commence avec seulement les cartes de base
        FillMagazine(); // Remplit le chargeur immédiatement au début
        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(false); // Cache la barre au début
        }
    }

    void Update()
    {   
        if (magazine.Count == 0)
            {
                StartCoroutine(ReloadMagazine());
                return;
            }
        else if (Input.GetMouseButtonDown(0) && enableShooting)
        {      
            throwRandomCard();
        }
    }

    public void setEnableShooting(bool newState)
    {
        enableShooting = newState;
    }
    //LS
    void throwRandomCard()
    {
        

        // Prendre la première carte du chargeur et la lancer
        GameObject selectedCard = magazine[0];
        magazine.RemoveAt(0);

        GameObject newCard = Instantiate(selectedCard);
        newCard.transform.position = transform.position;
        newCard.transform.rotation = transform.rotation;

        FindFirstObjectByType<AudioManager>().playSound("Card Throw");
    }
    //LS
    IEnumerator ReloadMagazine()
    {
        if (isReloading) yield break; // Empêche un rechargement multiple

        isReloading = true;
        enableShooting = false;

        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(true); // Affiche la barre
            reloadBar.value = 0f; // Initialise la barre à 0
        }

        float elapsedTime = 0f;
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            if (reloadBar != null)
                reloadBar.value = elapsedTime / reloadTime; // Met à jour la progression

            yield return null;
        }

        magazine.Clear();
        for (int i = 0; i < magazineSize; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            magazine.Add(availableCards[randomIndex]);
        }

        Debug.Log("Magasin rechargé !");
        isReloading = false;
        enableShooting = true;

        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(false); // Cache la barre une fois le rechargement terminé
        }
    }

    //LS
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

    //LS
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
