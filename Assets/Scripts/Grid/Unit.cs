using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
//using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Unit : MonoBehaviour 
{
    protected Pathfinding.Heuristic heuristic = Pathfinding.Heuristic.TileDistance; //determine which heuristic to use
    public UnitTypes unitType;
    [SerializeField] protected float movementSpeed = 20;
    [SerializeField] protected bool canFly; //bool to toggle flying pathfinding
    [SerializeField] protected int unitPlayerId;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int damage;
    [SerializeField] protected int defense;
    [SerializeField] protected int minRange;
    [SerializeField] protected int maxRange;
    [SerializeField] protected int accuracy;
    [SerializeField] protected int evasion;
    [SerializeField] protected int cost;
    [SerializeField] protected Node[] path;
    [SerializeField] protected int targetIndex;
    public bool isClicked = false;

    [Header("UI")]
    [SerializeField] private Slider m_healthBar;

    private const int MAXValue = Int32.MaxValue;
    private const int MINValue = 0;
    private Camera mainCam;
    private const int constantMovementSpeed = 7;
    
    #region UnitModifications

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
    
    #endregion

    void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        m_healthBar.value = (float) currentHealth / maxHealth;

        if (currentHealth < 1)
        {
            Player owningPlayer = GameLoop.instance.GetPlayer(unitPlayerId);
            
            if (unitType == UnitTypes.King)
                owningPlayer.KingAlive = false;
            
            owningPlayer.RemoveUnit(this);
            Destroy(gameObject);
        }
    }

    public void SelectNewUnitPosition()
    {
        RaycastHit hit;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                // fixes out of bounce error that occurs when unit selected.
                if (Vector3.Distance(hit.point, transform.position) < 1)
                    return; // already at destination

                PathRequestManager.RequestPath(transform.position, hit.transform.position, canFly, this.GetUnitPlayerID(), OnPathFound, heuristic);
            }
        }
    }
    
    


    // passes this function when requesting for path
    // function starts the coroutine if pathfinding is successful
    public void OnPathFound(Node[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 1; //keeps track of the next node the unit is going to move to
            
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    //updates unit position by following along the path
    public IEnumerator FollowPath()
    {
        Node unitNode = path[0];//keeps track of the node the unit is currently on
        Node currentWaypoint = path[1]; //keeps track of the next node the unit is going to move to

        //Grid grid = GameObject.Find("Pathfinding").GetComponent<Grid>();

        while (true)
        {
            //grid.NodeFromWorldPoint(transform.position).RemoveUnit(this);
            unitNode.RemoveUnit(this);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.worldPosition, constantMovementSpeed * Time.deltaTime);

            if (transform.position == currentWaypoint.worldPosition) 
            {
                targetIndex++; //it starts from 1 now
                currentWaypoint.AddUnit(this);
                CheckHostileTrapOrItemInNode(currentWaypoint); // moved this

                if (targetIndex >= path.Length) 
                    yield break;

                currentWaypoint = path[targetIndex];
                unitNode = path[targetIndex - 1];
                //currentWaypoint.RemoveUnit(this);// moved this
                //currentWaypoint = path[targetIndex];
                //currentWaypoint.AddUnit(this);
                //CheckHostileTrapOrItemInNode(currentWaypoint); // moved this
            }

            //transform.position = Vector3.MoveTowards(transform.position,currentWaypoint.worldPosition,constantMovementSpeed * Time.deltaTime);

            //CheckHostileTrapOrItemInNode(currentWaypoint);

            yield return null;
        }
    }
    
    public IEnumerator AIFollowPath(Node[] newPath)
    {
        if (newPath.Length < 2)
            yield break;
        
        Node unitNode = newPath[0];//keeps track of the node the unit is currently on
        Node currentWaypoint = newPath[1]; //keeps track of the next node the unit is going to move to

        //Grid grid = GameObject.Find("Pathfinding").GetComponent<Grid>();

        while (true)
        {
            //grid.NodeFromWorldPoint(transform.position).RemoveUnit(this);
            unitNode.RemoveUnit(this);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.worldPosition, constantMovementSpeed * Time.deltaTime);

            if (transform.position == currentWaypoint.worldPosition) 
            {
                targetIndex++; //it starts from 1 now
                currentWaypoint.AddUnit(this);
                CheckHostileTrapOrItemInNode(currentWaypoint); // moved this

                if (targetIndex >= newPath.Length) 
                    yield break;

                currentWaypoint = newPath[targetIndex];
                unitNode = newPath[targetIndex - 1];
                //currentWaypoint.RemoveUnit(this);// moved this
                //currentWaypoint = path[targetIndex];
                //currentWaypoint.AddUnit(this);
                //CheckHostileTrapOrItemInNode(currentWaypoint); // moved this
            }

            //transform.position = Vector3.MoveTowards(transform.position,currentWaypoint.worldPosition,constantMovementSpeed * Time.deltaTime);

            //CheckHostileTrapOrItemInNode(currentWaypoint);

            yield return null;
        }
    }

    public void CheckHostileTrapOrItemInNode(Node waypoint)
    {
        List<TrapOrItem> toiList = waypoint.GetTrapOrItemList();

        foreach (TrapOrItem toi in toiList.ToArray())
            toi.TrapOrItemTriggeredByUnit(waypoint);

        // removed if statement to make player trigger their own traps
        /*{
            if (toi.GetTrapOrItemPlayerID() != this.GetUnitPlayerID())
            {
                toi.TrapOrItemTriggeredByUnit();
            }
        }*/
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

/*    void OnMouseDown()
    {
        Debug.Log("Player ID: " + GetComponent<Unit>().GetUnitPlayerID() +
            "\nType: " + GetComponent<Unit>().GetUnitType() +
            "\nMax HP: " + GetComponent<Unit>().GetMaxHealth() +
            "\nCurrent HP: " + GetComponent<Unit>().GetCurrentHealth() +
            "\nDamage: " + GetComponent<Unit>().GetDamage() +
            "\nDefence: " + GetComponent<Unit>().GetDefence() +
            "\nMin Range: " + GetComponent<Unit>().GetMinRange() +
            "\nMax Range: " + GetComponent<Unit>().GetMaxRange() +
            "\nAccuracy: " + GetComponent<Unit>().GetAccuracy() +
            "\nEvasion: " + GetComponent<Unit>().GetEvasion() +
            "\nMS: " + GetComponent<Unit>().GetMovementSpeed() +
            "\nFlying: " + GetComponent<Unit>().GetCanFly());
    }*/
}