using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardsData", menuName = "Cards/Cards Data", order = 1)]
[Serializable]
public class CardsData : ScriptableObject
{
    [System.Serializable]
    public class CardEntry
    {
        public int cardId;                // Unique identifier for the card
        public GameObject objectToSpawn;  // Prefab to spawn when the card is dragged
        public Sprite cardImage;          // Image displayed on the card
    }

    public List<CardEntry> cardEntries = new List<CardEntry>();  // List of card data entries
}