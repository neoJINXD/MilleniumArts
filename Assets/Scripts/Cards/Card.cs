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

public enum CastType
{
    OnAlly = 0,
    OnEnemy = 1,
    OnEmpty = 2,
    OnAny = 3
}

[Serializable]
public class Card : MonoBehaviour
{

    public int id;
    public CardType type;
    public CastType castType;
    public string name;
    public GameObject image;
    public int cost;
    public int minRange;
    public int maxRange;

    public Card()
    {

    }

    public Card(int cardId, CardType cardType, CastType cardCastType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax)
    {
        id = cardId;
        type = cardType;
        castType = cardCastType;
        name = cardName;
        //image = cardImage;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
    }
}


