using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card
{
    public int health;
    public int damage;
    public int defence;
    public int minAttackRange;
    public int maxAttackRange;
    public int moveSpeed;
    public int accuracy;
    public int evasion;
    public bool flying;

    public UnitCard(int cardId, CastType cardCastType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax,
        int cardHealth, int cardDamage, int cardDefence, int cardMinARange, int cardMaxARange, int cardMoveSpeed, int cardAccuracy,
        int cardEvasion, bool cardFlying)
    {
        id = cardId;
        castType = cardCastType;
        name = cardName;
        //image = cardImage;
        cost = cardCost;
        minRange = cardMin;
        maxRange = cardMax;
        health = cardHealth;
        damage = cardDamage;
        defence = cardDefence;
        minAttackRange = cardMinARange;
        maxAttackRange = cardMaxARange;
        moveSpeed = cardMoveSpeed;
        accuracy = cardAccuracy;
        evasion = cardEvasion;
        flying = cardFlying;

    }

    public void copyUnitCard(UnitCard copyCard)
    {
        id = copyCard.id;
        castType = copyCard.castType;
        name = copyCard.name;
        //image = cardImage;
        cost = copyCard.cost;
        minRange = copyCard.minRange;
        maxRange = copyCard.maxRange;

        indexInHand = 0;

        health = copyCard.health;
        damage = copyCard.damage;
        defence = copyCard.defence;
        minAttackRange = copyCard.minAttackRange;
        maxAttackRange = copyCard.maxAttackRange;
        moveSpeed = copyCard.moveSpeed;
        accuracy = copyCard.accuracy;
        evasion = copyCard.evasion;
        flying = copyCard.flying;

    }
}