using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TinyWar;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;           // Reference to the Card prefab
    public Transform cardContainer;         // Where cards will be instantiated in the UI
    public CardsData cardsData;             // Reference to the CardsData ScriptableObject

    private List<Card> cards = new List<Card>();

    void Start()
    {
        GenerateCards();
    }

    // Generates initial cards
    void GenerateCards()
    {
        foreach (var cardEntry in cardsData.cardEntries)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            Card cardComponent = newCard.GetComponent<Card>();
            cardComponent.Setup(cardEntry, this); // Setup card using CardEntry data
            cards.Add(cardComponent);
        }
    }

    // Handles dragging and dropping the card
    public void CardDragEnded(Card card)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse position
        RaycastHit hit;

        // Perform the raycast to detect hit
        if (Physics.Raycast(ray, out hit))
        {
            // Successful hit, spawn object at the hit point
            GameObject spawnedObject = TinyWarManager.Instance.SpawnPlayerInGround(card.cardEntry.objectToSpawn);
            card.gameObject.SetActive(false); // Disable the dragged card after spawning
            CheckForMerge(card); // Check for card merge logic
        }
        else
        {
            // If no valid hit, reset the card to its initial position
            card.ReturnToInitialPosition();
        }
    }

    // Checks if two identical cards can merge
    public void CheckForMerge(Card draggedCard)
    {
        foreach (var card in cards)
        {
            if (card.cardEntry.cardId == draggedCard.cardEntry.cardId && card != draggedCard && !card.isMerged)
            {
                card.isMerged = true;
                draggedCard.isMerged = true;

                // Merge logic: create a new card or new object
                CreateMergedCard(draggedCard.cardEntry);
                return;
            }
        }
    }

    // Creates a merged card
    void CreateMergedCard(CardsData.CardEntry oldCardEntry)
    {
        int newCardId = Mathf.Min(oldCardEntry.cardId + 1, cardsData.cardEntries.Count - 1);
        CardsData.CardEntry newCardEntry = cardsData.cardEntries[newCardId];

        GameObject newCard = Instantiate(cardPrefab, cardContainer);
        Card cardComponent = newCard.GetComponent<Card>();
        cardComponent.Setup(newCardEntry, this);
        cards.Add(cardComponent);
    }
}