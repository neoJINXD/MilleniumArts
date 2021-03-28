using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Unit = 0,
    Spell = 1,
    Trap = 2
}

[Serializable]
public class Card
{
    public int ManaCost;
    public CardType Type;
}
