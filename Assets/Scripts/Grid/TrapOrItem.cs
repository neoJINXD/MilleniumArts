using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOrItem : MonoBehaviour
{
    protected int trapOrItemPlayerID;
    protected int trapOrItemHealth;
    protected int minRange;
    protected int maxRange;
    protected TrapOrItemTypes trapOrItemType;
    private const int MAXValue = Int32.MaxValue;
    private const int MINValue = 0;
    
    public enum TrapOrItemTypes //enum for item/trap types
    {
        BearTrap,
        LandMine,
        WoodenBarricade,
        StoneBarricade,
        MetalBarricade,
        ItemUndefined
    }
    
    //default abstract constructor
    protected TrapOrItem()
    {
        trapOrItemPlayerID = -1;
        trapOrItemHealth = 0;
        minRange = 0;
        maxRange = 0;
        trapOrItemType = TrapOrItemTypes.ItemUndefined;
    }

    //parameterized abstract constructor
    protected TrapOrItem(int _trapOrItemPlayerID, int _trapOrItemHealth, int _minRange, int _maxRange, TrapOrItemTypes _trapOrItemType)
    {
        trapOrItemPlayerID = _trapOrItemPlayerID;
        trapOrItemHealth = _trapOrItemHealth;
        minRange = _minRange;
        maxRange = _maxRange;
        trapOrItemType = _trapOrItemType;
    }

    //function called when Unit triggers the Trap or Item in a Node
    public virtual void TrapOrItemTriggeredByUnit()
    {
        //override per child class specification
        throw new Exception("Override TrapOrItemTriggeredByUnit function");
    }
    
    //set and get functions for item type
    public virtual void SetTrapOrItemType(TrapOrItemTypes toi)
    {
        trapOrItemType = toi;
    }
    
    public virtual TrapOrItemTypes GetItemType()
    {
        return trapOrItemType;
    }

    //set and get functions for item playerID
    public virtual void SetTrapOrItemPlayerID(int pID)
    {
        trapOrItemPlayerID = pID;
    }
    
    public virtual int GetTrapOrItemPlayerID()
    {
        return trapOrItemPlayerID;
    }
    
    //set, get and update functions for item health
    public virtual void SetTrapOrItemHealth(int iH)
    {
        trapOrItemHealth = Mathf.Clamp(iH, MINValue, MAXValue);
    }

    public virtual int GetTrapOrItemHealth()
    {
        return trapOrItemHealth;
    }

    public virtual int IncreaseTrapOrItemHealthBy(int iH)
    {
        trapOrItemHealth += iH;
        trapOrItemHealth = Mathf.Clamp(trapOrItemHealth, MINValue, MAXValue);
        return trapOrItemHealth;
    }
    
    public virtual int DecreaseTrapOrItemHealthBy(int iH)
    {
        trapOrItemHealth -= iH;
        trapOrItemHealth = Mathf.Clamp(trapOrItemHealth, MINValue, MAXValue);
        return trapOrItemHealth;
    }
    
    //set, get and update functions for min range
    public virtual void SetMinRange(int minR)
    {
        minRange = Mathf.Clamp(minR, MINValue, MAXValue);
    }

    public virtual int GetMinRange()
    {
        return minRange;
    }

    public virtual int IncreaseMinRangeBy(int minR)
    {
        minRange += minR;
        minRange = Mathf.Clamp(minRange, MINValue, MAXValue);
        return minRange;
    }
    
    public virtual int DecreaseMinRangeBy(int minR)
    {
        minRange -= minR;
        minRange = Mathf.Clamp(minRange, MINValue, MAXValue);
        return minRange;
    }
    
    //set, get and update functions for max range
    public virtual void SetMaxRange(int maxR)
    {
        maxRange = Mathf.Clamp(maxR, MINValue, MAXValue);
    }

    public virtual int GetMaxRange()
    {
        return maxRange;
    }

    public virtual int IncreaseMaxRangeBy(int maxR)
    {
        maxRange += maxR;
        maxRange = Mathf.Clamp(maxRange, MINValue, MAXValue);
        return maxRange;
    }
    
    public virtual int DecreaseMaxRangeBy(int maxR)
    {
        maxRange -= maxR;
        maxRange = Mathf.Clamp(maxRange, MINValue, MAXValue);
        return maxRange;
    }
}
