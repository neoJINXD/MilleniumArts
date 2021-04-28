using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
	public SpellCard() {}
	
	public string description;
	
    public SpellCard(int cardId, CastType cardCastType, string cardName, int cardCost, int cardMin, int cardMax, string cardDesc, int cardAOEMin, int cardAOEMax)
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

        description = cardDesc;
    }

    public void copySpellCard(SpellCard copyCard)
    {
        id = copyCard.id;
        castType = copyCard.castType;
        name = copyCard.name;
        cost = copyCard.cost;
        minRange = copyCard.minRange;
        maxRange = copyCard.maxRange;
        aoeMinRange = copyCard.aoeMinRange;
        aoeMaxRange = copyCard.aoeMaxRange;
        indexInHand = copyCard.indexInHand;

        description = copyCard.description;
    }
}