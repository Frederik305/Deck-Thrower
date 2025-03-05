using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// LS
public class LootScreen : MonoBehaviour
{
    // Liste des cartes débloquées
    public List<int> unlockedCards = new List<int>(); 
    // Référence au script Shoot pour gérer les actions liées aux cartes
    public Shoot shootScript; 
    // Préfabriqué pour une carte de butin
    public GameObject lootCard; 
    
    // Références pour les différents composants UI
    private Canvas canvasLoot;
    private Transform canvasLootTransform;
    private Transform magazineDisplay;
    
    // Listes pour stocker les instances de cartes affichées
    private List<GameObject> cardsInstance = new List<GameObject>();
    private List<GameObject> magazineCards = new List<GameObject>();
    
    private Canvas canvasMagazine;
    private int cardToAddIndex; // Index de la carte à ajouter

    // LS
    // Initialisation des composants UI et autres références
    void Start()
    {
        canvasLoot = gameObject.GetComponent<Canvas>();
        canvasLoot.enabled = false; // Cache l'écran de butin au départ
        canvasMagazine = GameObject.Find("Canvas Magazine").GetComponent<Canvas>();
        canvasLootTransform = GameObject.Find("CardsChoiceDisplay").GetComponent<Transform>();
        magazineDisplay = GameObject.Find("CardsDisplay").GetComponent<Transform>();
    }

    // LS
    // Active l'écran de butin et affiche les choix de cartes
    public void activate(GameObject[] cardTypes)
    {
        Time.timeScale = 0f; // Met le jeu en pause

        DisplayCardsChoice(); // Affiche les cartes à choisir
        DisplayMagazine();    // Affiche les cartes disponibles dans le magazine

        shootScript.setEnableShooting(false); // Désactive le tir
        canvasLoot.enabled = true;  // Active l'écran de butin
        canvasMagazine.enabled = false; // Cache le magazine
    }
    
    // LS
    // Affiche les cartes disponibles dans le magazine
    private void DisplayMagazine()
    {
        foreach (GameObject card in shootScript.availableCards)
        {
            SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();

            if (cardSpriteRenderer != null)
            {
                // Instancie une nouvelle carte dans le magazine
                GameObject magazineCard = Instantiate(lootCard, magazineDisplay);

                // Affecte l'image de la carte
                Image newCardImage = magazineCard.GetComponent<Image>();
                if (newCardImage != null)
                {
                    newCardImage.sprite = cardSpriteRenderer.sprite;
                    newCardImage.preserveAspect = true;
                }

                // Désactive le bouton et l'ajoute à la liste des cartes du magazine
                magazineCard.GetComponent<Button>().onClick.AddListener(() => ChangeCard(card));
                magazineCard.GetComponent<Button>().enabled = false;
                magazineCards.Add(magazineCard);

                // Ajuste la taille de la carte
                magazineCard.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
        }
    }

    // LS
    // Change la carte sélectionnée dans le jeu
    void ChangeCard(GameObject card)
    {
        Time.timeScale = 1f; // Reprend le jeu
        canvasLoot.enabled = false; // Cache l'écran de butin
        shootScript.setEnableShooting(true); // Active à nouveau les tirs

        // Détruit les anciennes cartes affichées
        foreach (GameObject instance in cardsInstance)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        foreach (GameObject instance in magazineCards)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }

        // Vide les listes après suppression des instances
        cardsInstance.Clear();
        magazineCards.Clear();

        // Change la carte du joueur
        shootScript.SwitchCard(card, cardToAddIndex);

        // Affiche le magazine après avoir changé la carte
        canvasMagazine.enabled = true;
    }

    // LS
    // Affiche les cartes disponibles à choisir pour le joueur
    private void DisplayCardsChoice()
    {
        // Mélange les cartes et prend les 3 premières
        List<(int index, GameObject card)> indexedCards = shootScript.cards
            .Select((card, index) => (index, card))  // Associe chaque carte à son index
            .OrderBy(_ => Random.value)  // Mélange la liste de manière aléatoire
            .Take(3)  // Sélectionne les 3 premières cartes
            .ToList();

        // Affiche chaque carte sélectionnée
        foreach (var item in indexedCards)
        {
            GameObject card = Instantiate(lootCard, canvasLootTransform);
            cardsInstance.Add(card);

            Image cardImage = card.GetComponent<Image>();
            cardImage.sprite = item.card.GetComponent<SpriteRenderer>().sprite;
            cardImage.preserveAspect = true; // Préserve le ratio d'aspect

            // Ajoute un bouton pour sélectionner la carte
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(card, item.index));
            card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }
    }

    // LS
    // Sélectionne une carte dans l'écran de butin
    public void SelectCard(GameObject clickedCard, int index)
    {
        cardToAddIndex = index; // Met à jour l'index de la carte sélectionnée

        // Réduit la taille de toutes les autres cartes
        foreach (GameObject card in cardsInstance)
        {
            if (card != clickedCard)
            {
                card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
        }

        // Agrandit la carte sélectionnée
        clickedCard.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // Active les boutons des cartes dans le magazine
        foreach (GameObject card in magazineCards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.enabled = true; // Active le bouton de la carte
            }
        }
    }
}
