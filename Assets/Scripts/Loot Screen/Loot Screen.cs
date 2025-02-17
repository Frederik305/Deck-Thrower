using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LootScreen : MonoBehaviour
{
    public List<int> unlockedCards = new List<int>(); // Liste des cartes débloquées
    public Shoot shootScript; // Référence au script Shoot

    void Start()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void activate(GameObject[] cardTypes)
    {
        GameObject cardHolder = gameObject.transform.GetChild(0).gameObject;

        List<int> lockedCards = Enumerable.Range(0, cardTypes.Length)
                                          .Where(index => !unlockedCards.Contains(index))
                                          .ToList();

        if (lockedCards.Count < 3)
        {
            Debug.LogWarning("Pas assez de cartes verrouillées disponibles !");
            return;
        }

        lockedCards = lockedCards.OrderBy(_ => Random.value).Take(3).ToList();

        cardHolder.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            GameObject card = cardHolder.transform.GetChild(i).gameObject;
            int index = lockedCards[i];

            card.GetComponent<Image>().sprite = cardTypes[index].GetComponent<SpriteRenderer>().sprite;
            card.GetComponent<LootOption>().setTypeIndex(index);

            // Ajoute un bouton pour débloquer la carte
            int selectedCardIndex = index;
            card.GetComponent<Button>().onClick.AddListener(() => UnlockCard(cardTypes[selectedCardIndex]));
        }
    }

    public void UnlockCard(GameObject newCard)
    {
    }
}
