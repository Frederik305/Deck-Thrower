using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardsMagazine : MonoBehaviour
{
    public Transform transform;
    public GameObject cardModel;
    public List<GameObject> cards = new List<GameObject>(); // Change array to List
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
   public void DisplayCardsMagazine(List<GameObject> magazine){
        ClearCardsMagazine();
        magazine.Reverse();
        foreach (GameObject card in magazine)
        {
            SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();

            if (cardSpriteRenderer != null)
            {
                // Instantiate the card model (empty object that will hold the model)
                GameObject newCard = Instantiate(cardModel, transform);

                // Get the SpriteRenderer component of the instantiated model
                Image newCardImage = newCard.GetComponent<Image>();

                if (newCardImage != null)
                {
                    // Apply the sprite from the card's SpriteRenderer to the new card model's SpriteRenderer
                    newCardImage.sprite = cardSpriteRenderer.sprite;
                }
                // Add the new card model to the list of cards
                cards.Add(newCard);
            }
            
        }
        magazine.Reverse();
    }
    public void RemoveLastCard(){
        if (cards.Count > 0)
        {
            GameObject lastCard = cards[cards.Count - 1]; // Get the last card
            cards.RemoveAt(cards.Count - 1); // Remove the last card from the list
            Destroy(lastCard); // Destroy the last card GameObject
        }
    }
    public void ClearCardsMagazine()
    {
        // Détruire toutes les cartes actuellement affichées dans le magazine
        foreach (GameObject card in cards)
        {
            Destroy(card); // Détruire le GameObject de la carte
        }

        // Vider la liste des cartes
        cards.Clear();
    }
}
