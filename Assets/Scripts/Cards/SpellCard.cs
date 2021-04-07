using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public string description;

    public SpellCard(int cardId, CastType cardCastType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax, string cardDesc, int cardAOEMin, int cardAOEMax)
    {
        id = cardId;
        castType = cardCastType;
        name = cardName;
        //image = cardImage;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
        aoeMinRange = cardAOEMin;
        aoeMinRange = cardAOEMax;
        description = cardDesc;
    }
}