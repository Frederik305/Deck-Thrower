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
    private List<GameObject> cardsInstance = new List<GameObject>();

    void Start()
    {
        canvasLoot=gameObject.GetComponent<Canvas>();
        canvasLootTransform = gameObject.GetComponent<Transform>();
        unlockedCards.Add(0);
        //gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    //LS
    public void activate(GameObject[] cardTypes)
    {
        List<int> lockedCards = Enumerable.Range(0, cardTypes.Length)
                                              .Where(index => !unlockedCards.Contains(index))
                                              .ToList();
        Debug.Log(lockedCards.Count);
        if (lockedCards.Count>0) {
            Time.timeScale = 0f;




            shootScript.setEnableShooting(false);
            canvasLoot.enabled = true;

            lockedCards = lockedCards.OrderBy(_ => Random.value).Take(3).ToList();
            int cardLockedToDisplay = lockedCards.Count() < 3 ? lockedCards.Count : 3;


            for (int i = 0; i < cardLockedToDisplay; i++)
            {
                GameObject card = Instantiate(lootCard, canvasLootTransform);
                cardsInstance.Add(card);

                int index = lockedCards[i];

                card.GetComponent<Image>().sprite = cardTypes[index].GetComponent<SpriteRenderer>().sprite;
                card.GetComponent<LootOption>().setTypeIndex(index);

                // Ajoute un bouton pour débloquer la carte
                int selectedCardIndex = index;
                card.GetComponent<Button>().onClick.AddListener(() => UnlockCard(cardTypes[selectedCardIndex], index));
            }
        }
    }
    //LS
    public void UnlockCard(GameObject newCard,int index)
    {
        unlockedCards.Add(index);
        shootScript.UnlockCard(newCard);
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
