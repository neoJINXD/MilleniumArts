using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CastType
{
    OnUnit = 0,
    OnEmpty = 1,
    OnAny = 2
}

[Serializable]
public class Card : MonoBehaviour
{
    public int id;
    public CastType castType;
    public string name;
    public GameObject image;
    public int cost;
    public int minRange;
    public int maxRange;
    public int aoeMinRange;
    public int aoeMaxRange;

    public int indexInHand;

    public Card() {}

    public Card(int cardId, CastType cardCastType, string cardName, int cardCost, int cardMin, int cardMax, int cardAOEMin, int cardAOEMax)
    {
        id = cardId;
        castType = cardCastType;
        name = cardName;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
        aoeMinRange = cardAOEMin;
        aoeMinRange = cardAOEMax;
        indexInHand = 0;
    }
}


