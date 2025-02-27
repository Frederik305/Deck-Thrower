using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LootScreen : MonoBehaviour
{
    public List<int> unlockedCards = new List<int>(); // Liste des cartes débloquées
    public Shoot shootScript; // Référence au script Shoot
    public GameObject lootCard;
    private Canvas canvasLoot;
    private Transform canvasLootTransform;
    private Transform magazineDisplay;
    private List<GameObject> cardsInstance = new List<GameObject>();
    private List<GameObject> magazineCards = new List<GameObject>();
    private Canvas canvasMagazine;
    private int cardToAddIndex;


    void Start()
    {
        
        canvasLoot=gameObject.GetComponent<Canvas>();
        canvasLoot.enabled=false;
        canvasMagazine = GameObject.Find("Canvas Magazine").GetComponent<Canvas>();
        canvasLootTransform = GameObject.Find("CardsChoiceDisplay").GetComponent<Transform>();
        magazineDisplay=GameObject.Find("CardsDisplay").GetComponent<Transform>();
        

    }
    //LS
    public void activate(GameObject[] cardTypes)
    {


        
        Time.timeScale = 0f;


        DisplayCardsChoice();
        DisplayMagazine();

        shootScript.setEnableShooting(false);
        canvasLoot.enabled = true;
        canvasMagazine.enabled = false;

            
            
            
        
    }
    //LS
    private void DisplayMagazine(){
        foreach (GameObject card in shootScript.availableCards)
        {
            SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();

            if (cardSpriteRenderer != null)
            {
                
                GameObject magazineCard = Instantiate(lootCard, magazineDisplay);

                
                Image newCardImage = magazineCard.GetComponent<Image>();

                if (newCardImage != null)
                {
                   
                    newCardImage.sprite = cardSpriteRenderer.sprite;
                    newCardImage.preserveAspect = true;
                }
                magazineCard.GetComponent<Button>().onClick.AddListener(() => ChangeCard(card));
                magazineCard.GetComponent<Button>().enabled = false;
                magazineCards.Add(magazineCard);
                magazineCard.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
            

        }
    }
    //LS
    void ChangeCard(GameObject card){
        
        Time.timeScale = 1f;
        canvasLoot.enabled = false;
        shootScript.setEnableShooting(true);

        foreach (GameObject instance in cardsInstance)
        {
            if (instance != null)
            {
                Destroy(instance); // Détruire chaque instance
            }
        }
        foreach (GameObject instance in magazineCards)
        {
            if (instance != null)
            {
                Destroy(instance); // Détruire chaque instance
            }
        }
        // Vider la liste après avoir détruit toutes les instances
        cardsInstance.Clear();
        magazineCards.Clear();
        shootScript.SwitchCard(card,cardToAddIndex);
        canvasMagazine.enabled = true;
    }
    //LS
    private void DisplayCardsChoice() {
        

        List<(int index, GameObject card)> indexedCards = shootScript.cards
            .Select((card, index) => (index, card))  // Associe chaque carte à son index dans la liste
            .OrderBy(_ => Random.value)  // Mélange la liste
            .Take(3)  // Sélectionne les 3 premières cartes
            .ToList();

        foreach (var item in indexedCards)
        {
            GameObject card = Instantiate(lootCard, canvasLootTransform);
            cardsInstance.Add(card);

            Image cardImage = card.GetComponent<Image>();
            cardImage.sprite = item.card.GetComponent<SpriteRenderer>().sprite;
            cardImage.preserveAspect = true; // Préserver le ratio d'aspect
            

            // Ajoute un bouton pour débloquer la carte
            
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(card,item.index));
            card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }
    }
    //LS
    public void SelectCard(GameObject clickedCard,int index)
    {
        cardToAddIndex=index;
        
        // Réduit la taille de toutes les cartes
        foreach (GameObject card in cardsInstance)
        {
            if (card != clickedCard)
            {
                // Réduit la taille des autres cartes
                card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
        }

        // Agrandit légèrement la carte cliquée
        clickedCard.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        foreach (GameObject card in magazineCards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.enabled = true;  // Activer le bouton
            }
        }
    }
}
