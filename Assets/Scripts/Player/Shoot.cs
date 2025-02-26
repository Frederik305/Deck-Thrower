using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{

    public GameObject[] cards;
    //public GameObject baseCards; // Cartes de base
    
    public List<GameObject> availableCards= new List<GameObject>();
    
    public int magazineSize = 6; // Nombre de cartes dans le chargeur
    private List<GameObject> magazine = new List<GameObject>();

    public GameObject icon;
    public float iconSelectedSize = 1.1f;
    public float iconWidth = 40f;

    public CardsMagazine cardsMagazine;
    private bool enableShooting = true;

    public float reloadTime = 2f; // Temps de rechargement en secondes
    private bool isReloading = false;
    public Slider reloadBar; // Référence à la barre de progression UI

    void Start()
    {
        
        
        FillMagazine(); // Remplit le chargeur immédiatement au début
        
    }
    //LS

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
        cardsMagazine.RemoveLastCard();

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

        ShuffleDeck(availableCards);
        FillMagazine();
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

        // Mélanger les cartes disponibles
        

        // Ajouter les cartes mélangées au magazine
        for (int i = 0; i < magazineSize; i++)
        {
            // Assure-toi que la liste availableCards contient suffisamment d'éléments
            if (i < availableCards.Count)
            {
                magazine.Add(availableCards[i]);
            }
            else
            {
                Debug.LogWarning("Il n'y a pas assez de cartes dans availableCards pour remplir le magazine !");
                break;
            }
        }
        Debug.Log("Chargeur initial rempli !");
        cardsMagazine.DisplayCardsMagazine(magazine);
    }

    //LS
    public void SwitchCard(GameObject cardToDrop, int cardToAddIndex)
    {
        CancelReloading();
        int index = availableCards.IndexOf(cardToDrop);
        availableCards[index] = cards[cardToAddIndex];
        

        FillMagazine();

    }

    //LS
    void ShuffleDeck(List<GameObject> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void CancelReloading()
    {
        if (isReloading)
        {
            StopCoroutine(ReloadMagazine());
            isReloading = false;
            enableShooting = true;

            if (reloadBar != null)
            {
                reloadBar.gameObject.SetActive(false); // Cacher la barre de progression
            }

            Debug.Log("Rechargement annulé !");
        }
    }


}
