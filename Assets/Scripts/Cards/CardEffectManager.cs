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
    // Card ID = 0
    // create Soldier unit
    public void createSoldierUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Soldier.");
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

        soldierUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            soldierUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            soldierUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // Card ID = 1
    // create Knight unit
    public void createKnightUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Knight.");

        Unit knightUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

        knightUnit.SetMovementSpeed(3);
        knightUnit.SetCanFly(false);
        knightUnit.SetUnitType(Unit.UnitTypes.Knight);
        knightUnit.SetUnitPlayerID(playerId);
        knightUnit.SetMaxHealth(20);
        knightUnit.SetCurrentHealth(20);
        knightUnit.SetDamage(7);
        knightUnit.SetDefence(3);
        knightUnit.SetMinRange(1);
        knightUnit.SetMaxRange(1);
        knightUnit.SetAccuracy(70);
        knightUnit.SetEvasion(10);

        selectedNode.AddUnit(knightUnit.GetComponent<Unit>());

        knightUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            knightUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            knightUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // Card ID = 2
    // create Assassin unit
    public void createAssassinUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Assassin.");

        Unit assassinUnit = Object.Instantiate(unitCreation, selectedNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

        assassinUnit.SetMovementSpeed(6);
        assassinUnit.SetCanFly(false);
        assassinUnit.SetUnitType(Unit.UnitTypes.Assassin);
        assassinUnit.SetUnitPlayerID(playerId);
        assassinUnit.SetMaxHealth(15);
        assassinUnit.SetCurrentHealth(15);
        assassinUnit.SetDamage(9);
        assassinUnit.SetDefence(0);
        assassinUnit.SetMinRange(1);
        assassinUnit.SetMaxRange(1);
        assassinUnit.SetAccuracy(95);
        assassinUnit.SetEvasion(60);

        selectedNode.AddUnit(assassinUnit);

        assassinUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            assassinUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            assassinUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // Card ID = 3
    // create Priest unit
    public void createPriestUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Priest.");
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

        priestUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            priestUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            priestUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // Card ID = 4
    // create Archer unit
    public void createArcherUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Archer.");
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

        archerUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            archerUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            archerUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }

    // Card ID = 5
    // create Dragon Rider unit
    public void createDragonRiderUnit(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Unit: Dragon Rider.");
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

        dragonRiderUnit.CheckHostileTrapOrItemInNode(selectedNode);

        //hardcoded color for test
        if (playerId == 0)
            dragonRiderUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            dragonRiderUnit.transform.GetComponent<Renderer>().material.color = Color.red;
    }


    /*
     * Card ID: 6
     * Card Name: Smite
     * Type: Spell(Active)
     * Cast Range: (1,1) tiles from a friendly unit
     * Effect: Damages an enemy unit for 5 health.
     * Cost: 2
     */
    public void spell_smite(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Smite!");
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() != playerId) // confirm that the unit is not ours
            {
                selectedNode.unitInThisNode.DecreaseCurrentHealthBy(5); // smite damage
                print("Card Effect Sucessful: Smite");
            }
        }
    }

    /*
     * Card ID: 7
     * Card Name: Snipe
     * Type: Spell(Active)
     * Cast Range: (1,3) tiles from a friendly unit
     * Effect: Damages an enemy unit for 10 health.
     * Cost: 3
     */
    public void spell_snipe(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Snipe!");
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() != playerId) // confirm that the unit is not ours
            {
                selectedNode.unitInThisNode.DecreaseCurrentHealthBy(10); // smite damage
                print("Card Effect Sucessful: Smite");
            }
        }
    }

    /*
     * Card ID: 8
     * Card Name: Heavenly Smite
     * Type: Spell(Active)
     * Cast Range: (0, 3) tiles from a friendly unit
     * Effect: Damages all enemies within (0,1) tiles of the casting origin for 3 health.
     * Cost: 5
     */
    public void spell_heavenlySmite(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        print("Player " + playerId + " used Heavenly Smite!");

        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 1);
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
     * Card ID: 9
     * Card Name: Prayer
     * Type: Spell(Active)
     * Cast Range: (0, 2) tiles from a friendly priest
     * Effect: Heal all allies within (0,1) tiles of the casting origin for 3 health.
     * Cost: 4
     */
    public void spell_prayer(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        print("Player " + playerId + " used Prayer!");

        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 1);
        HashSet<Node> getAllyNodesInRange = new HashSet<Node>();

        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                    getAllyNodesInRange.Add(node);
            }
        }

        foreach (Node node in getAllyNodesInRange)
        {
            node.GetUnit().IncreaseCurrentHealthBy(3);
            print("Card Effect Sucessful: Prayer");
        }
    }

    /*
     * Card ID: 10
     * Card Name: Vitality
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s current and maximum Health by 5.
     * Cost: 1
     */
    public void spell_vitality(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Vitality!");

        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseMaxHealthBy(5);
                selectedNode.unitInThisNode.IncreaseCurrentHealthBy(5);
                print("Card Effect Sucessful: Vitality");
            }
        }
    }

    /*
     * Card ID: 11
     * Card Name: Endurance
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Defence by 2.
     * Cost: 2
     */
    public void spell_endurance(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Endurance!");

        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseDefenceBy(2);
                print("Card Effect Sucessful: Endurance");
            }
        }
    }

    /*
     * Card ID: 12
     * Card Name: Vigor
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Damage by 3.
     * Cost: 2
     */
    public void spell_vigor(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Vigor!");
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseDamageBy(3);
                print("Card Effect Sucessful: Vigor");
            }
        }
    }

    /*
     * Card ID: 13
     * Card Name: Nimbleness
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s movement speed by 1.
     * Cost: 2
     */
    public void spell_nimbleness(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Nimbleness!");

        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseMovementSpeedBy(1);
                print("Card Effect Sucessful: Nimbleness");
            }
        }
    }

    /*
     * Card ID: 14
     * Card Name: Agility
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s Evasion by 10.
     * Cost: 2
     */
    public void spell_agility(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Agility!");
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseEvasionBy(10);
                print("Card Effect Sucessful: Agility");
            }
        }
    }

    /*
     * Card ID: 15
     * Card Name: Precision
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Increases an ally unit’s current and Accuracy by 10.
     * Cost: 2
     */
    public void spell_precision(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Precision!");

        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.unitInThisNode.IncreaseAccuracyBy(10);
                print("Card Effect Sucessful: Precision");
            }
        }
    }

    /*
     * Card ID: 16
     * Card Name: Oracle
     * Type: Spell(Trap)
     * Cast Range: (0, 5) tiles from any friendly unit
     * Effect: Reveals all traps within (0,2) tiles of the triggering origin
     * Cost: 2
     */
    public void Oracle(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        print("Player " + playerId + " used Oracle!");

        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 2);

        foreach(Node node in allNodesInRange)
        {
            if(node.GetTrapOrItemList().Count > 0)
            {
                foreach(TrapOrItem toi in node.GetTrapOrItemList())
                {
                    if (toi.GetTrapOrItemPlayerID() != playerId)
                        print("There is an enemy trap on (" + node.gridX + ", " + node.gridY + ").");
                }
            }
        }
    }

    /*
     * Card ID: 17
     * Card Name: Disarm Trap
     * Type: Spell(Trap)
     * Cast Range: (1, 3) tiles from any friendly unit
     * Effect: Attempts to disarm an enemy trap on a tile.
     * Cost: 1
     */
    public void disarmTrap(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        print("Player " + playerId + " used Disarm Trap!");

        if (selectedNode.GetTrapOrItemList().Count > 0)
        {
            foreach (TrapOrItem toi in selectedNode.GetTrapOrItemList())
            {
                if (toi.GetTrapOrItemPlayerID() != playerId)
                {
                    selectedNode.RemoveTrapOrItem(toi);
                    print("Enemy trapped removed from tile (" + selectedNode.gridX + ", " + selectedNode.gridY + ").");
                }
            }
        }
    }

    /*
     * Card ID: 18
     * Card Name: Provisions
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Heals a unit for 5 health.
     * Cost: 1
     */
    public void spell_provisions(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Provisions!");

        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            if (selectedNode.GetUnit().GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                selectedNode.GetUnit().IncreaseCurrentHealthBy(5);
                print("Card Effect Sucessful: Provisions");
            }
        }
    }

    /*
     * Card ID: 19
     * Card Name: Reinforcements
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Summons 4 Soldier units around an ally.
     * Cost: 6
     */
    public void spell_reinforcements(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 1, 1);

        int unitCountAroundOrigin = 0;
        foreach(Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
                unitCountAroundOrigin++;
        }

        if(unitCountAroundOrigin == 0)
        {
            foreach (Node node in allNodesInRange)
                createSoldierUnit(playerId, node);
            print("Player " + playerId + " used Provisions!");
        }
        else
            print("Surrounding tiles are occupied. Cannot cast Reinforcements.");

    }

    /*
     * Card ID: 20
     * Card Name: Greed
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Draw 2 cards.
     * Cost: 3
     */
    public void spell_greed(int playerId)
    {
        // draw 2 cards
    }

    /*
     * Card ID: 21
     * Card Name: Warcry
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from a friendly Knight
     * Effect: Increases the damage of all allies within (0,1) tiles of the casting origin by 2
     * Cost: 4
     */
    public void spell_warcry(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 1, 1);

        int allyCountAroundOrigin = 0;
        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                    allyCountAroundOrigin++;
            }
        }

        if (allyCountAroundOrigin == 0)
        {
            foreach (Node node in allNodesInRange)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                    selectedNode.unitInThisNode.IncreaseDamageBy(2);
            }
            print("Player " + playerId + " used Warcry!");
        }
        else
            print("No allies surrounding the origin!");
    }

    /*
     * Card ID: 22
     * Card Name: Rebirth
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Fully heals an ally unit
     * Cost: 4
     */
    public void spell_rebirth(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Rebirth!");

        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() == playerId) // confirm that the unit is ours
            {
                if (selectedNode.unitInThisNode.GetCurrentHealth() != selectedNode.unitInThisNode.GetMaxHealth())
                {
                    selectedNode.unitInThisNode.SetCurrentHealth(selectedNode.unitInThisNode.GetMaxHealth());
                    print("Card Effect Sucessful: Provisions");
                }
            }
        }
    }

    /*
     * Card ID: 23
     * Card Name: Assassinate
     * Type: Spell(Active)
     * Cast Range: (1,1) tiles from any friendly Assassin unit
     * Effect: Damages an enemy unit for 5 health. Does not work on King unit.
     * Cost: 5
     */
    public void spell_assassinate(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Assassinate!");
        if (selectedNode.unitInThisNode != null) // check if there's a unit on this node
        {
            if (selectedNode.unitInThisNode.GetUnitPlayerID() != playerId && selectedNode.unitInThisNode.GetUnitType() != Unit.UnitTypes.King) // confirm that the unit is not ours
            {
                selectedNode.RemoveUnit(selectedNode.unitInThisNode); // smite damage
                print("Card Effect Sucessful: Assassinate");
            }
        }
    }

    /*
     * Card ID: 24
     * Card Name: Teleport
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from any friendly unit
     * Effect: Teleports a unit (1,2) tiles. 
     * Cost: 2
     */
    public void spell_teleport(int playerId, Node selectedNode)
    {
        // teleport unit
    }

    /*
     * Card ID: 25
     * Card ID: 
     * Card Name: Bear Trap
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages the triggering unit for 5 health.
     * Cost: 3
     */
    public void spell_bearTrap(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Bear Trap!");

        if (selectedNode.unitInThisNode == null) // check if there's a unit on this node
        {
            TrapOrItem bearTrap = new TrapOrItem(playerId, 0, 0, 0, TrapOrItem.TrapOrItemTypes.BearTrap);
            selectedNode.AddTrapOrItem(bearTrap);

            print("Card Effect Sucessful: Bear Trap");

        }
    }

    /*
     * Card ID: 26
     * Card Name: Land Mine
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages the triggering unit for 5 health.
     * Cost: 3
     */
    public void spell_landMine(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Land Mine!");

        if (selectedNode.unitInThisNode == null) // check if there's a unit on this node
        {
            TrapOrItem landMine = new TrapOrItem(playerId, 0, 0, 0, TrapOrItem.TrapOrItemTypes.LandMine);
            selectedNode.AddTrapOrItem(landMine);

            print("Card Effect Sucessful: Land Mine");

        }
    }
}

