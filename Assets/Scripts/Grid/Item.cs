using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected int itemPlayerID;
    protected int itemHealth;
    protected int minRange;
    protected int maxRange;
    protected ItemTypes itemType;
    private const int MAXValue = Int32.MaxValue;
    private const int MINValue = 0;
    
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
        itemPlayerID = -1;
        itemHealth = 0;
        minRange = 0;
        maxRange = 0;
        itemType = ItemTypes.ItemUndefined;
    }

    protected Item(int _itemPlayerID, int _itemHealth, int _minRange, int _maxRange, ItemTypes _itemType)
    {
        itemPlayerID = _itemPlayerID;
        itemHealth = _itemHealth;
        minRange = _minRange;
        maxRange = _maxRange;
        itemType = _itemType;
    }
    
    public virtual void SetItemType(ItemTypes iT)
    {
        itemType = iT;
    }
    
    public virtual ItemTypes GetItemType()
    {
        return itemType;
    }

    public virtual void SetItemPlayerID(int pID)
    {
        itemPlayerID = pID;
    }
    
    public virtual int GetItemPlayerID(int pID)
    {
        return itemPlayerID;
    }
    
    public virtual void SetItemHealth(int iH)
    {
        itemHealth = Mathf.Clamp(iH, MINValue, MAXValue);
    }

    public virtual int GetItemHealth()
    {
        return itemHealth;
    }

    public virtual int IncreaseItemHealth(int iH)
    {
        itemHealth += iH;
        itemHealth = Mathf.Clamp(itemHealth, MINValue, MAXValue);
        return itemHealth;
    }
    
    public virtual int DecreaseItemHealth(int iH)
    {
        itemHealth -= iH;
        itemHealth = Mathf.Clamp(itemHealth, MINValue, MAXValue);
        return itemHealth;
    }
    
    public virtual void SetMinRange(int minR)
    {
        minRange = Mathf.Clamp(minR, MINValue, MAXValue);
    }

    public virtual int GetMinRange()
    {
        return minRange;
    }

    public virtual int IncreaseMinRange(int minR)
    {
        minRange += minR;
        minRange = Mathf.Clamp(minRange, MINValue, MAXValue);
        return minRange;
    }
    
    public virtual int DecreaseMinRange(int minR)
    {
        minRange -= minR;
        minRange = Mathf.Clamp(minRange, MINValue, MAXValue);
        return minRange;
    }
    
    public virtual void SetMaxRange(int maxR)
    {
        maxRange = Mathf.Clamp(maxR, MINValue, MAXValue);
    }

    public virtual int GetMaxRange()
    {
        return maxRange;
    }

    public virtual int IncreaseMaxRange(int maxR)
    {
        maxRange += maxR;
        maxRange = Mathf.Clamp(maxRange, MINValue, MAXValue);
        return maxRange;
    }
    
    public virtual int DecreaseMaxRange(int maxR)
    {
        maxRange -= maxR;
        maxRange = Mathf.Clamp(maxRange, MINValue, MAXValue);
        return maxRange;
    }
}