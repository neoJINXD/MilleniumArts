using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect
{
    // What the effect does
    EffectType type;
    // Stat affected, if any
    Stat stat;
    // How much the effect changes things
    int param;
    //TODO: GRID SPACE PARAMETER FOR AREA OF EFFECT
}
enum EffectType {
    Heal,
    Damage,
    Buff,
    Debuff,
    Trap,
    Obstacle,
    AreaOfEffect,
    Move,
    Reveal
}
enum Stat {
    Health,
    Damage,
    AttackRange,
    MovementSpeed,
    Accuracy,
    Evasion
}