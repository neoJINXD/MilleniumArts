using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject unitCreation;

    private bool placerClicked = false;
    private const float lockAxis = 27f;

    private HashSet<Node> validMove;
    private Pathfinding pathfinding;
    private Unit currentUnit;
    private Unit[] unitArray;

    void Awake()
    {
        pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // create King unit
    public void createKingUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit kingUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        kingUnit.SetMovementSpeed(5);
        kingUnit.SetCanFly(false);
        kingUnit.SetUnitType(Unit.UnitTypes.King);
        kingUnit.SetUnitPlayerID(playerId);
        kingUnit.SetMaxHealth(30);
        kingUnit.SetCurrentHealth(30);
        kingUnit.SetDamage(7);
        kingUnit.SetDefence(2);
        kingUnit.SetMinRange(1);
        kingUnit.SetMaxRange(1);
        kingUnit.SetAccuracy(90);
        kingUnit.SetEvasion(30);

        //hardcoded color for test
        if (playerId == 0)
            kingUnit.transform.GetComponent<Renderer>().material.color =  Color.blue;
        else
            kingUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Soldier unit
    public void createSoldierUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit soldierUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        soldierUnit.SetMovementSpeed(4);
        soldierUnit.SetCanFly(false);
        soldierUnit.SetUnitType(Unit.UnitTypes.Soldier);
        soldierUnit.SetUnitPlayerID(playerId);
        soldierUnit.SetMaxHealth(10);
        soldierUnit.SetCurrentHealth(10);
        soldierUnit.SetDamage(5);
        soldierUnit.SetDefence(1);
        soldierUnit.SetMinRange(1);
        soldierUnit.SetMaxRange(1);
        soldierUnit.SetAccuracy(80);
        soldierUnit.SetEvasion(20);
    }

    // create Knight unit
    public void createKnightUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit knightUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        knightUnit.SetMovementSpeed(3);
        knightUnit.SetCanFly(false);
        knightUnit.SetUnitType(Unit.UnitTypes.Knight);
        knightUnit.SetUnitPlayerID(playerId);
        knightUnit.SetMaxHealth(30);
        knightUnit.SetCurrentHealth(30);
        knightUnit.SetDamage(7);
        knightUnit.SetDefence(1);
        knightUnit.SetMinRange(1);
        knightUnit.SetMaxRange(1);
        knightUnit.SetAccuracy(70);
        knightUnit.SetEvasion(10);
    }

    // create Assassin unit
    public void createAssassinUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit assassinUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        assassinUnit.SetMovementSpeed(8);
        assassinUnit.SetCanFly(false);
        assassinUnit.SetUnitType(Unit.UnitTypes.Assassin);
        assassinUnit.SetUnitPlayerID(playerId);
        assassinUnit.SetMaxHealth(30);
        assassinUnit.SetCurrentHealth(30);
        assassinUnit.SetDamage(9);
        assassinUnit.SetDefence(0);
        assassinUnit.SetMinRange(1);
        assassinUnit.SetMaxRange(1);
        assassinUnit.SetAccuracy(95);
        assassinUnit.SetEvasion(60);
    }

    // create Priest unit
    public void createPriestUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit priestUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        priestUnit.SetMovementSpeed(4);
        priestUnit.SetCanFly(false);
        priestUnit.SetUnitType(Unit.UnitTypes.Priest);
        priestUnit.SetUnitPlayerID(playerId);
        priestUnit.SetMaxHealth(15);
        priestUnit.SetCurrentHealth(15);
        priestUnit.SetDamage(5);
        priestUnit.SetDefence(0);
        priestUnit.SetMinRange(0);
        priestUnit.SetMaxRange(2);
        priestUnit.SetAccuracy(100);
        priestUnit.SetEvasion(30);
    }

    // create Archer unit
    public void createArcherUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit archerUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        archerUnit.SetMovementSpeed(4);
        archerUnit.SetCanFly(false);
        archerUnit.SetUnitType(Unit.UnitTypes.Archer);
        archerUnit.SetUnitPlayerID(playerId);
        archerUnit.SetMaxHealth(15);
        archerUnit.SetCurrentHealth(15);
        archerUnit.SetDamage(6);
        archerUnit.SetDefence(0);
        archerUnit.SetMinRange(2);
        archerUnit.SetMaxRange(3);
        archerUnit.SetAccuracy(90);
        archerUnit.SetEvasion(30);
    }

    // create Dragon Rider unit
    public void createDragonRiderUnit(int playerId)
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));

        Unit dragonRiderUnit = Object.Instantiate(unitCreation, areaToInstantiate, Quaternion.identity).GetComponent<Unit>();

        dragonRiderUnit.SetMovementSpeed(6);
        dragonRiderUnit.SetCanFly(false);
        dragonRiderUnit.SetUnitType(Unit.UnitTypes.DragonRider);
        dragonRiderUnit.SetUnitPlayerID(playerId);
        dragonRiderUnit.SetMaxHealth(25);
        dragonRiderUnit.SetCurrentHealth(25);
        dragonRiderUnit.SetDamage(6);
        dragonRiderUnit.SetDefence(2);
        dragonRiderUnit.SetMinRange(1);
        dragonRiderUnit.SetMaxRange(1);
        dragonRiderUnit.SetAccuracy(85);
        dragonRiderUnit.SetEvasion(20);
    }



    /*
     * Card Name: Smite
     * Type: Spell(Active)
     * Cast Range: (1,1) tiles from a friendly unit
     * Effect: Damages an enemy unit for 5 health.
     * Cost: 3
     */
    public void spell_smite(int playerId)
    {
        Grid grid = pathfinding.grid;
        List<Node> allyNodes = grid.GetAllyUnitNodes(0);
        print("XD: " + allyNodes.Count);
        foreach(Node node in allyNodes)
        {
            print(node.GetUnitList());
        }
    }
}

