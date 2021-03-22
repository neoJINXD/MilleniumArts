using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected int itemHealth;
    protected int minRange;
    protected int maxRange;
    protected ItemTypes itemType;
    
    public enum ItemTypes //enum for item/trap types
    {
        BearTrap,
        LandMine,
        WoodenBarricade,
        StoneBarricade,
        MetalBarricade,
        ItemUndefined
    }
    
    //default abstract constructor
    protected Item()
    {
        itemHealth = 0;
        minRange = 0;
        maxRange = 0;
        itemType = ItemTypes.ItemUndefined;
    }

    protected Item(int _itemHealth, int _minRange, int _maxRange, ItemTypes _itemType)
    {
        itemHealth = _itemHealth;
        minRange = _minRange;
        maxRange = _maxRange;
        itemType = _itemType;
    }
    
}