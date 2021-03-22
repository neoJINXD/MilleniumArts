using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class Unit : MonoBehaviour {

    [SerializeField] protected float movementSpeed = 20;
    [SerializeField] protected bool canFly; //bool to toggle flying pathfinding
    protected Pathfinding.Heuristic heuristic = Pathfinding.Heuristic.TileDistance; //determine which heuristic to use
    protected UnitTypes type;
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
        type = UnitTypes.Undefined;
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
        type = _type;
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