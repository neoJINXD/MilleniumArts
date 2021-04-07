using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject unitCreation;

    private bool placerClicked = false;
    private const float lockAxis = 27f;

    private HashSet<Node> validMove;
    private Pathfinding pf;
    private Unit currentUnit;
    private Unit[] unitArray;

    private Grid grid;

    void Awake()
    {
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        grid = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // create Soldier unit
    public void createSoldierUnit(int playerId, Node selectedNode)
    {
        Unit soldierUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(soldierUnit);

        //hardcoded color for test
        if (playerId == 0)
            soldierUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            soldierUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Knight unit
    public void createKnightUnit(int playerId, Node selectedNode)
    {
        Unit knightUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(knightUnit.GetComponent<Unit>());

        //hardcoded color for test
        if (playerId == 0)
            knightUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            knightUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Assassin unit
    public void createAssassinUnit(int playerId, Node selectedNode)
    {
        Unit assassinUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(assassinUnit);

        //hardcoded color for test
        if (playerId == 0)
            assassinUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            assassinUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Priest unit
    public void createPriestUnit(int playerId, Node selectedNode)
    {
        Unit priestUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(priestUnit);

        //hardcoded color for test
        if (playerId == 0)
            priestUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            priestUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Archer unit
    public void createArcherUnit(int playerId, Node selectedNode)
    {
        Unit archerUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(archerUnit);

        //hardcoded color for test
        if (playerId == 0)
            archerUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            archerUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // create Dragon Rider unit
    public void createDragonRiderUnit(int playerId, Node selectedNode)
    {
        Unit dragonRiderUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

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

        selectedNode.AddUnit(dragonRiderUnit);

        //hardcoded color for test
        if (playerId == 0)
            dragonRiderUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            dragonRiderUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }


    /*
     * Card Name: Smite
     * Type: Spell(Active)
     * Cast Range: (1,1) tiles from a friendly unit
     * Effect: Damages an enemy unit for 5 health.
     * Cost: 3
     */
    public void spell_smite(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() != playerId) // confirm that the unit is not ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.DecreaseCurrentHealthBy(5); // smite damage
                print("Card Effect Sucessful: Smite");
            }
        }
    }

    /*
     * Card Name: Heavenly Smite
     * Type: Spell(Active)
     * Cast Range: (0, 3) tiles from a friendly unit
     * Effect: Damages all enemies within (0,1) tiles of the casting origin for 3 health.
     * Cost: 3
     */
    public void spell_heavenlySmite(int playerId, Node selectedNode, Vector3 nodePosition)
    {
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(nodePosition, false, 0, 1);
        HashSet<Node> getEnemyNodesInRange = new HashSet<Node>();

        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() != playerId)
                    getEnemyNodesInRange.Add(node);
            }
        }

        foreach (Node node in getEnemyNodesInRange)
        {
            node.GetUnit().DecreaseCurrentHealthBy(3);
            print("Card Effect Sucessful: Heavenly Smite");
        }
    }

    /*
     * Card Name: Vitality
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s current and maximum Health by 5.
     * Cost: 3
     */
    public void spell_vitality(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseMaxHealthBy(5);
                targetUnit.IncreaseCurrentHealthBy(5);
                print("Card Effect Sucessful: Vitality");
            }
        }
    }

    /*
     * Card Name: Endurance
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Defence by 2.
     * Cost: 2
     */
    public void spell_endurance(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseDefenceBy(2);
                print("Card Effect Sucessful: Endurance");
            }
        }
    }

    /*
     * Card Name: Vigor
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Damage by 3.
     * Cost: 2
     */
    public void spell_vigor(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            print("1");
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseDamageBy(3);
                print("Card Effect Sucessful: Vigor");
            }
        }
    }

    /*
     * Card Name: Nimbleness
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s movement speed by 1.
     * Cost: 2
     */
    public void spell_nimbleness(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseMovementSpeedBy(1);
                print("Card Effect Sucessful: Nimbleness");
            }
        }
    }

    /*
     * Card Name: Agility
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Evasion by 10.
     * Cost: 2
     */
    public void spell_agility(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseEvasionBy(10);
                print("Card Effect Sucessful: Agiliy");
            }
        }
    }

    /*
     * Card Name: Precision
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s current and Accuracy by 10.
     * Cost: 2
     */
    public void spell_precision(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                Unit targetUnit = selectedNode.unitInThisNode;
                targetUnit.IncreaseAccuracyBy(10);
                print("Card Effect Sucessful: Precision");
            }
        }
    }

    /*
     * Card Name: Bear Trap
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages the triggering unit for 5 health.
     * Cost: 3
     */
    public void spell_bearTrap(int playerId, Node selectedNode)
    {
        if (selectedNode.unitInThisNode == null) // check if there's a unit on this node
        {
            TrapOrItem bearTrap = new TrapOrItem(playerId, 0, 1, 1, TrapOrItem.TrapOrItemTypes.BearTrap);
            selectedNode.AddTrapOrItem(bearTrap);

            print("Card Effect Sucessful: Bear Trap");

        }
    }
}

