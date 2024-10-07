using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TinyWar;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;           // Reference to the Card prefab
    public Transform cardContainer;         // Where cards will be instantiated in the UI
    public CardsData cardsData;             // Reference to the CardsData ScriptableObject

    [SerializeField]
    private List<CardsData.CardEntry> cardDeck = new List<CardsData.CardEntry>();

    private List<Card> cards = new List<Card>();


    void Start()
    {
        GenerateCards();
    }

    // Generates initial cards
    void GenerateCards()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            foreach (var cardEntry in cardsData.cardEntries)
            {
                if (cardEntry.cardId == cardDeck[i].cardId)
                {
                    cardDeck[i] = cardEntry;
                    GameObject newCard = Instantiate(cardPrefab, cardContainer);
                    Card cardComponent = newCard.GetComponent<Card>();
                    cardComponent.Setup(cardEntry, this); // Setup card using CardEntry data
                    cards.Add(cardComponent);
                }
            }
        }
    }

    // Handles dragging and dropping the card
    public void CardDragEnded(Card card)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse position
        RaycastHit hit;

        // Perform the raycast to detect hit
        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Platform"))
        {

            // Successful hit, spawn object at the hit point
            GameObject spawnedObject = TinyWarManager.Instance.SpawnPlayerInGround(card.cardEntry.objectToSpawn, hit.point);
            card.gameObject.SetActive(false); // Disable the dragged card after spawning

        }
        else
        {
            Card cardUnderPointer = GetCardUnderPointer(card);

            if (cardUnderPointer != null && cardUnderPointer.cardEntry.cardId == card.cardEntry.cardId)
            {
                // Merge logic if card IDs match
                MergeCards(card, cardUnderPointer);
            }

            else
            {
                // Reset the card if no merge happens
                card.ReturnToInitialPosition();
            }
        }
    }



    // Checks if the pointer is currently over a UI elemen

    // Detects the card under the pointer using Raycast
    private Card GetCardUnderPointer(Card card)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Card cardHit = result.gameObject.GetComponent<Card>();

            if (cardHit != null && cardHit != card)
            {
                return cardHit;
            }
        }

        return null;
    }

    private void MergeCards(Card draggedCard, Card targetCard)
    {
        int targetCardsiblingIndex = cards.IndexOf(targetCard);
        cards.Remove(draggedCard);
        cards.Remove(targetCard);
        cardDeck.Remove(draggedCard.cardEntry);
        cardDeck.Remove(targetCard.cardEntry);

        // Disable both cards and create a merged card
        draggedCard.gameObject.SetActive(false);
        targetCard.gameObject.SetActive(false);

        CreateMergedCard(targetCard, targetCardsiblingIndex);
    }

    // Creates a new merged card
    void CreateMergedCard(Card targetCard, int targetCardsiblingIndex)
    {
        int newCardId = targetCard.cardEntry.cardId * 10 + targetCard.cardEntry.cardId;

        foreach (var cardEntry in cardsData.cardEntries)
        {
            if (cardEntry.cardId == newCardId)
            {
                cardDeck.Insert(targetCardsiblingIndex, cardEntry);
                GameObject newCard = Instantiate(cardPrefab, cardContainer);
                newCard.transform.SetSiblingIndex(targetCardsiblingIndex);
                Card cardComponent = newCard.GetComponent<Card>();
                cardComponent.Setup(cardEntry, this); // Setup card using CardEntry data
                cards.Insert(targetCardsiblingIndex,cardComponent);
            }
        }
        

    }
}