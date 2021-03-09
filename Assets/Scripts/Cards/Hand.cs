using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    // The cards in the hand
    List<Card> handContent;

    // Adds a card to the hand
    public void addToHand(Card c) {
        handContent.Add(c);
    }

    // Plays a card from the hand
    void playFromHand(int i) {
        handContent.RemoveAt(i);
        // TODO: PERFORM CARD ACTION
    }

    // The number of cards in the hand
    int getHandSize() {
        return handContent.Count;
    }
}
