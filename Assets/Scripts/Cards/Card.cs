using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Unit = 0,
    Active = 1,
    Trap = 2
}

[Serializable]
public class Card
{
    public int id;
    public CardType type;
    public string name;
    public GameObject image;
    public int cost;
    public int minRange;
    public int maxRange;

    public Card()
    {

    }

    public Card(int cardId, CardType cardType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax)
    {
        id = cardId;
        type = cardType;
        name = cardName;
        //image = cardImage;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
    }
}
