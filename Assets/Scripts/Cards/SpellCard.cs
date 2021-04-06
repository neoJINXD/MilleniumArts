using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public string description;

    public SpellCard(int cardId, CardType cardType, CastType cardCastType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax, string cardDesc)
    {
        id = cardId;
        type = cardType;
        castType = cardCastType;
        name = cardName;
        //image = cardImage;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
        description = cardDesc;
    }
}