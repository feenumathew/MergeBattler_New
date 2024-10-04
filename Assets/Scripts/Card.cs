using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector3 initialPosition;         // Store initial position of the card
    public CardsData.CardEntry cardEntry;   // Link to CardEntry from CardsData ScriptableObject
    public bool isMerged = false;
    private CardManager cardManager;
    private CanvasGroup canvasGroup;

    public Image cardImage;                 // Reference to the UI Image component of the card

    void Start()
    {
        initialPosition = transform.position; // Set initial position
    }

    // Set up the card using CardEntry from CardsData ScriptableObject
    public void Setup(CardsData.CardEntry entry, CardManager manager)
    {
        cardEntry = entry;
        cardManager = manager;
        canvasGroup = GetComponent<CanvasGroup>();
        cardImage = GetComponent<Image>();
        UpdateCardUI(); // Update appearance based on cardEntry
    }

    // Updates the card's UI (e.g., image) based on cardEntry
    void UpdateCardUI()
    {
        cardImage.sprite = cardEntry.cardImage; // Set the image from the cardEntry
    }

    // Dragging logic
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.position;   // Save the initial position when the drag starts
        canvasGroup.alpha = 0.6f;               // Make the card semi-transparent while dragging
        canvasGroup.blocksRaycasts = false;     // Disable raycasting for the card while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Move the card with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;               // Return transparency to normal
        canvasGroup.blocksRaycasts = true;      // Enable raycasting again

        // Inform CardManager that this card has been dragged
        cardManager.CardDragEnded(this);
    }

    // Method to return the card to its initial position if no valid spawn point is found
    public void ReturnToInitialPosition()
    {
        transform.position = initialPosition;   // Return card to its initial position
    }
}