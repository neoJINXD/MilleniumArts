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

    public UnitCard(int cardId, CardType cardType, string cardName, /*GameObject cardImage,*/ int cardCost, int cardMin, int cardMax,
        int cardHealth, int cardDamage, int cardDefence, int cardMinARange, int cardMaxARange, int cardMoveSpeed, int cardAccuracy,
        int cardEvasion, bool cardFlying)
    {
        id = cardId;
        type = cardType;
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
}