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

    public void copySpellCard(SpellCard copyCard)
    {
        id = copyCard.id;
        castType = copyCard.castType;
        name = copyCard.name;
        //image = cardImage;
        cost = copyCard.cost;
        minRange = copyCard.minRange;
        maxRange = copyCard.maxRange;
        aoeMinRange = copyCard.aoeMinRange;
        aoeMaxRange = copyCard.aoeMaxRange;
        description = copyCard.description;
    }
}