using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // The cards in the deck
    Queue<Card> deckContent;

    // A reference to the hand the deck should draw to
    Hand playerHand;

    // Draws a card from the deck and adds it to the hand
    void draw() {
        Card drawn =  deckContent.Dequeue();
        playerHand.addToHand(drawn);
    }

    // The number of cards currently in the deck
    int getDeckSize() {
        return deckContent.Count;
    }

}
