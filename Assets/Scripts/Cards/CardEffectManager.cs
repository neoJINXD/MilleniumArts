using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class CardEffectManager : Singleton<CardEffectManager>
{
    [SerializeField] private Unit m_soldier;
    [SerializeField] private Unit m_knight;
    [SerializeField] private Unit m_assassin;
    [SerializeField] private Unit m_priest;
    [SerializeField] private Unit m_archer;
    [SerializeField] private Unit m_dragonRider;
    
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

    public void CreateUnit(Unit.UnitTypes unit, Node positionNode)
    {
        Unit placedUnit = null;
        
        if (unit == Unit.UnitTypes.Soldier)
            placedUnit = Instantiate(m_soldier.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
        if (unit == Unit.UnitTypes.Knight)
            placedUnit = Instantiate(m_knight.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
        if (unit == Unit.UnitTypes.Assassin)
            placedUnit = Instantiate(m_assassin.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
        if (unit == Unit.UnitTypes.Priest)
            placedUnit = Instantiate(m_priest.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
        if (unit == Unit.UnitTypes.Archer)
            placedUnit = Instantiate(m_archer.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
        if (unit == Unit.UnitTypes.DragonRider)
            placedUnit = Instantiate(m_dragonRider.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();

        int playerId = GameLoop.instance.GetCurrentPlayer().PlayerId;
        placedUnit.SetUnitPlayerID(playerId);

        positionNode.AddUnit(placedUnit);

        //hardcoded color for test
        if (playerId == 0)
            placedUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            placedUnit.transform.GetComponent<Renderer>().material.color = Color.red;
        
        GameLoop.instance.GetCurrentPlayer().AddUnit(placedUnit);

        placedUnit.CheckHostileTrapOrItemInNode(positionNode);

        print(unit + " unit placed");
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
            TurnManager.instance.cardSuccessful = true;
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
            TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
    public void spell_oracle(int playerId, Node selectedNode, Vector3 selectedNodePosition)
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

        TurnManager.instance.cardSuccessful = true;
    }

    /*
     * Card ID: 17
     * Card Name: Disarm Trap
     * Type: Spell(Trap)
     * Cast Range: (1, 3) tiles from any friendly unit
     * Effect: Attempts to disarm an enemy trap on a tile.
     * Cost: 1
     */
    public void spell_disarmTrap(int playerId, Node selectedNode, Vector3 selectedNodePosition)
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
        TurnManager.instance.cardSuccessful = true;
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
                TurnManager.instance.cardSuccessful = true;
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
                CreateUnit(Unit.UnitTypes.Soldier, node);
            TurnManager.instance.cardSuccessful = true;
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
        TurnManager.instance.cardSuccessful = true;
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
            TurnManager.instance.cardSuccessful = true;
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
                    TurnManager.instance.cardSuccessful = true;
                    print("Card Effect Sucessful: Rebirth");
                }
            }
        }
    }

    /*
     * Card ID: 23
     * Card Name: Assassinate
     * Type: Spell(Active)
     * Cast Range: (1,1) tiles from any friendly Assassin unit
     * Effect: Immediately kills and enemy unit. Does not work on King unit.
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
                TurnManager.instance.cardSuccessful = true;
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
        TurnManager.instance.cardSuccessful = true;
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

            TurnManager.instance.cardSuccessful = true;
            print("Card Effect Sucessful: Bear Trap");

        }
    }

    /*
     * Card ID: 26
     * Card Name: Land Mine
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages all units from (0,1) tiles from the detonation origin for 3 health.
     * Cost: 3
     */
    public void spell_landMine(int playerId, Node selectedNode)
    {
        print("Player " + playerId + " used Land Mine!");

        if (selectedNode.unitInThisNode == null) // check if there's a unit on this node
        {
            TrapOrItem landMine = new TrapOrItem(playerId, 0, 0, 0, TrapOrItem.TrapOrItemTypes.LandMine);
            selectedNode.AddTrapOrItem(landMine);

            TurnManager.instance.cardSuccessful = true;
            print("Card Effect Sucessful: Land Mine");

        }
    }

    /*
     * Card ID: 27
     * Card Name: Royal Pledge
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from a friendly King
     * Effect: Increases the damage, current health and maximum health of all allies within (1,1) tiles of the casting origin by 2 
     * Cost: 4
     */
    public void spell_royalPledge(int playerId, Node selectedNode, Vector3 selectedNodePosition)
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
                {
                    selectedNode.unitInThisNode.IncreaseDamageBy(2);
                    selectedNode.unitInThisNode.IncreaseMaxHealthBy(2);
                    selectedNode.unitInThisNode.IncreaseCurrentHealthBy(2);
                }
            }
            TurnManager.instance.cardSuccessful = true;
            print("Player " + playerId + " used Royal Pledge!");
        }
        else
            print("No allies surrounding the origin!");
    }
}

