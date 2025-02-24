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
    public Transform canvasLootTransform;
    public Transform magazineDisplay;
    private List<GameObject> cardsInstance = new List<GameObject>();


    void Start()
    {
        canvasLoot=gameObject.GetComponent<Canvas>();
        canvasLootTransform = gameObject.GetComponent<Transform>();
        
        //gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    //LS
    public void activate(GameObject[] cardTypes)
    {


        
        Time.timeScale = 0f;


        DisplayCardsChoice();
        DisplayMagazine();

        shootScript.setEnableShooting(false);
        canvasLoot.enabled = true;

            
            
            
        
    }
    private void DisplayMagazine(){
        foreach (GameObject card in shootScript.availableCards)
        {
            SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();

            if (cardSpriteRenderer != null)
            {
                // Instantiate the card model (empty object that will hold the model)
                GameObject newCard = Instantiate(lootCard, magazineDisplay);

                // Get the SpriteRenderer component of the instantiated model
                Image newCardImage = newCard.GetComponent<Image>();

                if (newCardImage != null)
                {
                    // Apply the sprite from the card's SpriteRenderer to the new card model's SpriteRenderer
                    newCardImage.sprite = cardSpriteRenderer.sprite;
                }
                
                
            }

        }
    }
    private void DisplayCardsChoice() {
        int cardLockedToDisplay =  3;

        lockedCards = cards.OrderBy(_ => Random.value).Take(3).ToList();
        for (int i = 0; i < cardLockedToDisplay; i++)
        {
            GameObject card = Instantiate(lootCard, canvasLootTransform);
            cardsInstance.Add(card);

            //int index = lockedCards[i];

            card.GetComponent<Image>().sprite = cardTypes[index].GetComponent<SpriteRenderer>().sprite;
            card.GetComponent<LootOption>().setTypeIndex(index);

            // Ajoute un bouton pour débloquer la carte
            int selectedCardIndex = index;
            card.GetComponent<Button>().onClick.AddListener(() => UnlockCard(cardTypes[selectedCardIndex], index));
        }
    }

    //LS
    public void UnlockCard(GameObject newCard,int index)
    {
        unlockedCards.Add(index);
        //shootScript.UnlockCard(newCard);
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
        // Vider la liste après avoir détruit toutes les instances
        cardsInstance.Clear();

    }
}
