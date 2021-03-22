using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class Unit : MonoBehaviour {

    [SerializeField] protected float movementSpeed = 20;
    [SerializeField] protected bool canFly; //bool to toggle flying pathfinding
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
    protected Vector3[] path;
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
        Undefined
    }

    //default abstract constructor
    protected Unit()
    {
        movementSpeed = 10;
        canFly = false;
        heuristic = Pathfinding.Heuristic.TileDistance;
        unitType = UnitTypes.Undefined;
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

    public virtual void SetMovementSpeed(float s)
    {
        movementSpeed = Mathf.Clamp(s, MINValue, MAXValue);
    }

    public virtual float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public virtual float IncreaseMovementSpeed(float s)
    {
        movementSpeed += s;
        movementSpeed = Mathf.Clamp(movementSpeed, MINValue, MAXValue);
        return movementSpeed;
    }
    
    public virtual float DecreaseMovementSpeed(float s)
    {
        movementSpeed -= s;
        movementSpeed = Mathf.Clamp(movementSpeed, MINValue, MAXValue);
        return movementSpeed;
    }

    public virtual void SetCanFly(bool cF)
    {
        canFly = cF;
    }

    public virtual bool GetCanFly()
    {
        return canFly;
    }
    
    public virtual void SetUnitType(UnitTypes uT)
    {
        unitType = uT;
    }
    
    public virtual UnitTypes GetUnitType()
    {
        return unitType;
    }

    public virtual void SetUnitPlayerID(int pID)
    {
        unitPlayerId = pID;
    }
    
    public virtual int GetUnitPlayerID(int pID)
    {
        return unitPlayerId;
    }
    
    public virtual void SetMaxHealth(int mH)
    {
        maxHealth = Mathf.Clamp(mH, MINValue, MAXValue);
    }

    public virtual int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual int IncreaseMaxHealth(int mH)
    {
        maxHealth += mH;
        maxHealth = Mathf.Clamp(maxHealth, MINValue, MAXValue);
        return maxHealth;
    }
    
    public virtual int DecreaseMaxHealth(int mH)
    {
        maxHealth -= mH;
        maxHealth = Mathf.Clamp(maxHealth, MINValue, MAXValue);
        return maxHealth;
    }
    
    public virtual void SetCurrentHealth(int cH)
    {
        currentHealth = Mathf.Clamp(cH, MINValue, MAXValue);
    }

    public virtual int GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual int IncreaseCurrentHealth(int cH)
    {
        currentHealth += cH;
        currentHealth = Mathf.Clamp(currentHealth, MINValue, MAXValue);
        return currentHealth;
    }
    
    public virtual int DecreaseCurrentHealth(int cH)
    {
        currentHealth -= cH;
        currentHealth = Mathf.Clamp(currentHealth, MINValue, MAXValue);
        return currentHealth;
    }
    
    public virtual void SetDamage(int dmg)
    {
        damage = Mathf.Clamp(dmg, MINValue, MAXValue);
    }

    public virtual int GetDamage()
    {
        return damage;
    }

    public virtual int IncreaseDamage(int dmg)
    {
        damage += dmg;
        damage = Mathf.Clamp(damage, MINValue, MAXValue);
        return damage;
    }
    
    public virtual int DecreaseDamage(int dmg)
    {
        damage -= dmg;
        damage = Mathf.Clamp(damage, MINValue, MAXValue);
        return damage;
    }
    
    public virtual void SetDefence(int def)
    {
        defense = Mathf.Clamp(def, MINValue, MAXValue);
    }

    public virtual int GetDefence()
    {
        return defense;
    }

    public virtual int IncreaseDefence(int def)
    {
        defense += def;
        defense = Mathf.Clamp(defense, MINValue, MAXValue);
        return defense;
    }
    
    public virtual int DecreaseDefence(int def)
    {
        defense -= def;
        defense = Mathf.Clamp(defense, MINValue, MAXValue);
        return defense;
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
    
    public virtual void SetAccuracy(int ac)
    {
        accuracy = Mathf.Clamp(ac, MINValue, MAXValue);
    }

    public virtual int GetAccuracy()
    {
        return accuracy;
    }

    public virtual int IncreaseAccuracy(int ac)
    {
        accuracy += ac;
        accuracy = Mathf.Clamp(accuracy, MINValue, MAXValue);
        return accuracy;
    }
    
    public virtual int DecreaseAccuracy(int ac)
    {
        accuracy -= ac;
        accuracy = Mathf.Clamp(accuracy, MINValue, MAXValue);
        return accuracy;
    }
    
    public virtual void SetEvasion(int ev)
    {
        evasion = Mathf.Clamp(ev, MINValue, MAXValue);
    }

    public virtual int GetEvasion()
    {
        return evasion;
    }

    public virtual int IncreaseEvasion(int ev)
    {
        evasion += ev;
        evasion = Mathf.Clamp(evasion, MINValue, MAXValue);
        return evasion;
    }
    
    public virtual int DecreaseEvasion(int ev)
    {
        evasion -= ev;
        evasion = Mathf.Clamp(evasion, MINValue, MAXValue);
        return evasion;
    }
    
    public virtual void SetCost(int c)
    {
        cost = Mathf.Clamp(c, MINValue, MAXValue);
    }

    public virtual int GetCost()
    {
        return cost;
    }

    public virtual int IncreaseCost(int c)
    {
        cost += c;
        cost = Mathf.Clamp(cost, MINValue, MAXValue);
        return cost;
    }
    
    public virtual int DecreaseCost(int c)
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
                    
                  PathRequestManager.RequestPath(transform.position,hit.point, canFly, OnPathFound, heuristic);
              }
         }
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() 
    {
        if (isClicked)
        {
            Vector3 currentWaypoint = path[0];
            while (true) {
                if (transform.position == currentWaypoint) {
                    targetIndex++;
                    if (targetIndex >= path.Length) {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,movementSpeed * Time.deltaTime);
            
                yield return null;
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