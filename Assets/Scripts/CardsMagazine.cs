using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Cette classe gère l'affichage des cartes dans un magazine.
/// Elle permet d'afficher les cartes dans un ordre spécifique, de retirer la dernière carte et de nettoyer le magazine.
/// </summary>
public class CardsMagazine : MonoBehaviour
{
    // Le transform auquel les cartes seront attachées
    public Transform transform;

    // Le modèle de carte à instancier pour chaque carte
    public GameObject cardModel;

    // Liste des cartes actuellement affichées dans le magazine
    public List<GameObject> cards = new List<GameObject>(); // Changer le tableau en liste

    /// <summary>
    /// Affiche les cartes dans le magazine à partir d'une liste donnée.
    /// L'ordre des cartes dans la liste est inversé avant l'affichage.
    /// Chaque carte reçoit le sprite de la carte dans la liste.
    /// </summary>
    /// <param name="magazine">La liste des cartes à afficher dans le magazine.</param>
    public void DisplayCardsMagazine(List<GameObject> magazine)
    {
        // Effacer les cartes actuellement dans le magazine
        ClearCardsMagazine();

        // Inverser l'ordre des cartes pour les afficher du bas vers le haut
        magazine.Reverse();

        // Pour chaque carte dans la liste
        foreach (GameObject card in magazine)
        {
            // Récupérer le composant SpriteRenderer de la carte
            SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();

            // Vérifier si la carte possède un SpriteRenderer
            if (cardSpriteRenderer != null)
            {
                // Instancier le modèle de carte et l'attacher au transform de la classe
                GameObject newCard = Instantiate(cardModel, transform);

                // Récupérer le composant Image du modèle de carte instancié
                Image newCardImage = newCard.GetComponent<Image>();

                // Vérifier si le modèle a un composant Image
                if (newCardImage != null)
                {
                    // Appliquer le sprite de la carte à l'image du modèle
                    newCardImage.sprite = cardSpriteRenderer.sprite;
                }

                // Ajouter la nouvelle carte à la liste des cartes
                cards.Add(newCard);
            }
        }

        // Ré-inverser l'ordre des cartes pour revenir à l'ordre initial
        magazine.Reverse();
    }

    /// <summary>
    /// Retire la dernière carte du magazine.
    /// Si des cartes existent, la dernière est supprimée et son objet GameObject est détruit.
    /// </summary>
    public void RemoveLastCard()
    {
        // Vérifier s'il y a des cartes dans le magazine
        if (cards.Count > 0)
        {
            // Récupérer la dernière carte dans la liste
            GameObject lastCard = cards[cards.Count - 1];

            // Retirer la dernière carte de la liste
            cards.RemoveAt(cards.Count - 1);

            // Détruire l'objet GameObject de la dernière carte
            Destroy(lastCard);
        }
    }

    /// <summary>
    /// Efface toutes les cartes actuellement affichées dans le magazine.
    /// Cette méthode détruit tous les objets GameObject représentant les cartes et vide la liste.
    /// </summary>
    public void ClearCardsMagazine()
    {
        // Détruire chaque carte actuellement affichée dans le magazine
        foreach (GameObject card in cards)
        {
            Destroy(card); // Détruire l'objet GameObject de la carte
        }

        // Vider la liste des cartes
        cards.Clear();
    }
}
