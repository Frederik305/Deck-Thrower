using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Classe modifié par Logan pour le système de gestion du paquet de carte 
public class Shoot : MonoBehaviour
{
    public GameObject[] cards; // Tableau contenant les différentes cartes à lancer
    public List<GameObject> availableCards = new List<GameObject>(); // Liste des cartes disponibles pour être utilisées

    public int magazineSize = 6; // Taille du chargeur, nombre de cartes que le joueur peut avoir dans le chargeur
    private List<GameObject> magazine = new List<GameObject>(); // Liste des cartes actuellement dans le chargeur

    public GameObject icon; // Icône représentant la carte
    public float iconSelectedSize = 1.1f; // Taille de l'icône lorsqu'elle est sélectionnée
    public float iconWidth = 40f; // Largeur de l'icône

    public CardsMagazine cardsMagazine; // Référence à un autre script qui gère l'affichage du magazine de cartes
    private bool enableShooting = true; // Permet de contrôler si le joueur peut tirer ou non

    public float reloadTime = 2f; // Temps de rechargement en secondes
    private bool isReloading = false; // Indique si le rechargement est en cours
    public Slider reloadBar; // Barre de progression pour afficher l'avancement du rechargement

    // LS
    // Start est appelé avant la première mise à jour
    void Start()
    {
        FillMagazine(); // Remplie le chargeur immédiatement au début
    }

    // LS
    // Update est appelé une fois par frame
    void Update()
    {
        if (magazine.Count == 0) // Si le magazine est vide, commence le rechargement
        {
            StartCoroutine(ReloadMagazine());
            return;
        }
        else if (Input.GetMouseButtonDown(0) && enableShooting) // Si le joueur clique avec la souris et que le tir est autorisé
        {      
            throwRandomCard(); // Lancer une carte aléatoire
        }
    }
    // LS
    // Méthode pour activer ou désactiver le tir
    public void setEnableShooting(bool newState)
    {
        enableShooting = newState;
    }

    // LS
    // Méthode pour lancer une carte aléatoire
    void throwRandomCard()
    {
        // Prendre la première carte du chargeur et la lancer
        GameObject selectedCard = magazine[0];
        magazine.RemoveAt(0); // Retirer la carte du chargeur
        cardsMagazine.RemoveLastCard(); // Mettre à jour l'affichage du magazine

        // Créer une nouvelle carte à l'endroit du joueur et avec la même orientation
        GameObject newCard = Instantiate(selectedCard);
        newCard.transform.position = transform.position;
        newCard.transform.rotation = transform.rotation;

        // Jouer un son lorsque la carte est lancée
        FindFirstObjectByType<AudioManager>().playSound("Card Throw");
    }

    // LS
    // Coroutine pour gérer le rechargement du chargeur
    IEnumerator ReloadMagazine()
    {
        if (isReloading) yield break; // Empêche un rechargement multiple pendant qu'un autre est en cours

        isReloading = true;
        enableShooting = false; // Désactive le tir pendant le rechargement

        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(true); // Affiche la barre de progression
            reloadBar.value = 0f; // Initialise la barre à 0
        }

        float elapsedTime = 0f;
        while (elapsedTime < reloadTime) // Boucle jusqu'à la fin du temps de rechargement
        {
            elapsedTime += Time.deltaTime; // Incrémenter le temps écoulé
            if (reloadBar != null)
                reloadBar.value = elapsedTime / reloadTime; // Met à jour la barre de progression

            yield return null; // Attendre la fin du frame avant de continuer
        }

        // Mélanger les cartes disponibles et remplir le magazine
        ShuffleDeck(availableCards);
        FillMagazine();
        Debug.Log("Magasin rechargé !");
        isReloading = false;
        enableShooting = true;

        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(false); // Cache la barre de progression une fois le rechargement terminé
        }
    }

    // LS
    // Méthode pour remplir le magazine avec des cartes mélangées
    void FillMagazine()
    {
        magazine.Clear(); // Vider le magazine

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
        cardsMagazine.DisplayCardsMagazine(magazine); // Met à jour l'affichage du magazine
    }

    // LS
    // Méthode pour changer une carte dans le chargeur
    public void SwitchCard(GameObject cardToDrop, int cardToAddIndex)
    {
        CancelReloading(); // Annule le rechargement en cours
        int index = availableCards.IndexOf(cardToDrop); // Trouver l'index de la carte à remplacer
        availableCards[index] = cards[cardToAddIndex]; // Remplacer la carte dans la liste disponible

        FillMagazine(); // Met à jour le magazine avec la nouvelle carte
    }

    // LS
    // Méthode pour mélanger les cartes disponibles
    void ShuffleDeck(List<GameObject> list)
    {
        System.Random rng = new System.Random(); // Générateur de nombres aléatoires
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

    // LS
    // Méthode pour annuler un rechargement en cours
    public void CancelReloading()
    {
        if (isReloading)
        {
            StopCoroutine(ReloadMagazine()); // Arrêter la coroutine de rechargement
            isReloading = false;
            enableShooting = true; // Réactiver le tir

            if (reloadBar != null)
            {
                reloadBar.gameObject.SetActive(false); // Cacher la barre de progression
            }

            Debug.Log("Rechargement annulé !");
        }
    }
}
