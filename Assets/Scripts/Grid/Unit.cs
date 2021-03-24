using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public abstract class Unit : MonoBehaviour {

    [SerializeField] protected float movementSpeed = 20;
    [SerializeField] protected bool canFly; //bool to toggle flying pathfinding
    [SerializeField] private Material displayPath;
    
    protected Pathfinding.Heuristic heuristic = Pathfinding.Heuristic.TileDistance; //determine which heuristic to use
    protected UnitTypes unitType;
    protected int unitPlayerId;
    protected int maxHealth;
    protected int currentHealth;
    protected int damage;
    protected int defense;
    protected int minRange;
    protected int maxRange;
    protected int accuracy;
    protected int evasion;
    protected int cost;
    protected Node[] path;
    protected int targetIndex;
    private const int MAXValue = Int32.MaxValue;
    private const int MINValue = 0;
    public bool isClicked = false;

    public enum UnitTypes //enum for unit types
    {
        King,
        Soldier,
        Knight,
        Assassin,
        Priest,
        Archer,
        DragonRider,
        UnitUndefined
    }

    //default abstract constructor
    protected Unit()
    {
        movementSpeed = 10;
        canFly = false;
        heuristic = Pathfinding.Heuristic.TileDistance;
        unitType = UnitTypes.UnitUndefined;
        unitPlayerId = -1;
        maxHealth = 0;
        currentHealth = 0;
        damage = 0;
        defense = 0;
        minRange = 0;
        maxRange = 0;
        accuracy = 0;
        evasion = 0;
        cost = 0;
    }
    
    // parameterized abstract constructor
    protected Unit(float _movementSpeed, bool _canfly, Pathfinding.Heuristic _hf, UnitTypes _type, int _unitPlayerId, int _maxHealth,
        int _currentHealth, int _damage, int _defense, int _minRange, int _maxRange, int _accuracy, int _evasion, int _cost)
    {
        movementSpeed = _movementSpeed;
        canFly = _canfly;
        heuristic = _hf;
        unitType = _type;
        unitPlayerId = _unitPlayerId;
        maxHealth = _maxHealth;
        currentHealth = _currentHealth;
        damage = _damage;
        defense = _defense;
        minRange = _minRange;
        maxRange = _maxRange;
        accuracy = _accuracy;
        evasion = _evasion;
        cost = _cost;
    }

    // dictionary of heap index and unit itself.

    /*
    For testing:
    public Transform target;
    void Start() 
    {
        // PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
    }*/

    //set, get and update functions for movement speed
    public virtual void SetMovementSpeed(float s)
    {
        movementSpeed = Mathf.Clamp(s, MINValue, MAXValue);
    }

    public virtual float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public virtual float IncreaseMovementSpeedBy(float s)
    {
        movementSpeed += s;
        movementSpeed = Mathf.Clamp(movementSpeed, MINValue, MAXValue);
        return movementSpeed;
    }
    
    public virtual float DecreaseMovementSpeedBy(float s)
    {
        movementSpeed -= s;
        movementSpeed = Mathf.Clamp(movementSpeed, MINValue, MAXValue);
        return movementSpeed;
    }

    //set and get functions for canFly
    public virtual void SetCanFly(bool cF)
    {
        canFly = cF;
    }

    public virtual bool GetCanFly()
    {
        return canFly;
    }
    
    //set and get functions for unit type
    public virtual void SetUnitType(UnitTypes uT)
    {
        unitType = uT;
    }
    
    public virtual UnitTypes GetUnitType()
    {
        return unitType;
    }
    
    //set and get functions for unit playerID
    public virtual void SetUnitPlayerID(int pID)
    {
        unitPlayerId = pID;
    }
    
    public virtual int GetUnitPlayerID()
    {
        return unitPlayerId;
    }
    
    //set, get and update functions for max health
    public virtual void SetMaxHealth(int mH)
    {
        maxHealth = Mathf.Clamp(mH, MINValue, MAXValue);
    }

    public virtual int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual int IncreaseMaxHealthBy(int mH)
    {
        maxHealth += mH;
        maxHealth = Mathf.Clamp(maxHealth, MINValue, MAXValue);
        return maxHealth;
    }
    
    public virtual int DecreaseMaxHealthBy(int mH)
    {
        maxHealth -= mH;
        maxHealth = Mathf.Clamp(maxHealth, MINValue, MAXValue);
        return maxHealth;
    }
    
    //set, get and update functions for current health
    public virtual void SetCurrentHealth(int cH)
    {
        currentHealth = Mathf.Clamp(cH, MINValue, MAXValue);
    }

    public virtual int GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual int IncreaseCurrentHealthBy(int cH)
    {
        currentHealth += cH;
        currentHealth = Mathf.Clamp(currentHealth, MINValue, MAXValue);
        return currentHealth;
    }
    
    public virtual int DecreaseCurrentHealthBy(int cH)
    {
        currentHealth -= cH;
        currentHealth = Mathf.Clamp(currentHealth, MINValue, MAXValue);
        return currentHealth;
    }
    
    //set, get and update functions for damage
    public virtual void SetDamage(int dmg)
    {
        damage = Mathf.Clamp(dmg, MINValue, MAXValue);
    }

    public virtual int GetDamage()
    {
        return damage;
    }

    public virtual int IncreaseDamageBy(int dmg)
    {
        damage += dmg;
        damage = Mathf.Clamp(damage, MINValue, MAXValue);
        return damage;
    }
    
    public virtual int DecreaseDamageBy(int dmg)
    {
        damage -= dmg;
        damage = Mathf.Clamp(damage, MINValue, MAXValue);
        return damage;
    }
    
    //set, get and update functions for defense
    public virtual void SetDefence(int def)
    {
        defense = Mathf.Clamp(def, MINValue, MAXValue);
    }

    public virtual int GetDefence()
    {
        return defense;
    }

    public virtual int IncreaseDefenceBy(int def)
    {
        defense += def;
        defense = Mathf.Clamp(defense, MINValue, MAXValue);
        return defense;
    }
    
    public virtual int DecreaseDefenceBy(int def)
    {
        defense -= def;
        defense = Mathf.Clamp(defense, MINValue, MAXValue);
        return defense;
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
    
    //set, get and update functions for accuracy
    public virtual void SetAccuracy(int ac)
    {
        accuracy = Mathf.Clamp(ac, MINValue, MAXValue);
    }

    public virtual int GetAccuracy()
    {
        return accuracy;
    }

    public virtual int IncreaseAccuracyBy(int ac)
    {
        accuracy += ac;
        accuracy = Mathf.Clamp(accuracy, MINValue, MAXValue);
        return accuracy;
    }
    
    public virtual int DecreaseAccuracyBy(int ac)
    {
        accuracy -= ac;
        accuracy = Mathf.Clamp(accuracy, MINValue, MAXValue);
        return accuracy;
    }
    
    //set, get and update functions for evasion
    public virtual void SetEvasion(int ev)
    {
        evasion = Mathf.Clamp(ev, MINValue, MAXValue);
    }

    public virtual int GetEvasion()
    {
        return evasion;
    }

    public virtual int IncreaseEvasionBy(int ev)
    {
        evasion += ev;
        evasion = Mathf.Clamp(evasion, MINValue, MAXValue);
        return evasion;
    }
    
    public virtual int DecreaseEvasionBy(int ev)
    {
        evasion -= ev;
        evasion = Mathf.Clamp(evasion, MINValue, MAXValue);
        return evasion;
    }
    
    //set, get and update functions for cost
    public virtual void SetCost(int c)
    {
        cost = Mathf.Clamp(c, MINValue, MAXValue);
    }

    public virtual int GetCost()
    {
        return cost;
    }

    public virtual int IncreaseCostBy(int c)
    {
        cost += c;
        cost = Mathf.Clamp(cost, MINValue, MAXValue);
        return cost;
    }
    
    public virtual int DecreaseCostBy(int c)
    {
        cost -= c;
        cost = Mathf.Clamp(cost, MINValue, MAXValue);
        return cost;
    }

    
    public void SelectNewUnitPosition()
    {
        
         if (Input.GetMouseButtonDown(0)) 
         {
              RaycastHit hit;
              Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                

              if (Physics.Raycast(ray, out hit, Mathf.Infinity))
              {
                  // fixes out of bounce error that occurs when unit selected.
                  if (Vector3.Distance(hit.point, transform.position) < 1)
                         return; // already at destination
                    
                  PathRequestManager.RequestPath(transform.position,hit.point, canFly, this.GetUnitPlayerID(), OnPathFound, heuristic);
              }
         }
    }
    
    
    
    // passes this function when requesting for path
    // function starts the coroutine if pathfinding is successful
    public void OnPathFound(Node[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    

    //updates unit position by following along the path
    IEnumerator FollowPath() 
    {
        if (isClicked)
        {
            Node currentWaypoint = path[0];
            while (true)
            {
                if (transform.position == currentWaypoint.worldPosition) 
                {
                    targetIndex++;
                    currentWaypoint.RemoveUnit(this);
                    
                    if (targetIndex >= path.Length) 
                    {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                    currentWaypoint.AddUnit(this);
                }

                transform.position = Vector3.MoveTowards(transform.position,currentWaypoint.worldPosition,movementSpeed * Time.deltaTime);

                CheckHostileTrapOrItemInNode(currentWaypoint);

                yield return null;
            }
        }
    }

    public void CheckHostileTrapOrItemInNode(Node waypoint)
    {
        List<TrapOrItem> toiList = waypoint.GetTrapOrItemList();

        foreach (TrapOrItem toi in toiList)
        {
            if (toi.GetTrapOrItemPlayerID() != this.GetUnitPlayerID())
            {
                toi.TrapOrItemTriggeredByUnit();
            }
        }
    }

    // public void OnDrawGizmos() 
    // {
    //     if (path != null) {
    //         for (int i = targetIndex; i < path.Length; i++) {
    //             Gizmos.color = Color.black;
    //             Gizmos.DrawCube(path[i], Vector3.one * (1f - 0.3f));
    //
    //             if (i == targetIndex) {
    //                 Gizmos.DrawLine(transform.position, path[i]);
    //             }
    //             else {
    //                 Gizmos.DrawLine(path[i - 1], path[i]);
    //             }
    //         }
    //     }
    // }
}