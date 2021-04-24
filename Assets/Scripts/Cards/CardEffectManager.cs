using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

    #region Animations

    
    // Units
    [SerializeField] public Sprite sprite_archer_blue;
    [SerializeField] public Sprite sprite_archer_red;
    [SerializeField] public Sprite sprite_assassin_blue;
    [SerializeField] public Sprite sprite_assassin_red;
    [SerializeField] public Sprite sprite_dragonRider_blue;
    [SerializeField] public Sprite sprite_dragonRider_red;
    [SerializeField] public Sprite sprite_knight_blue;
    [SerializeField] public Sprite sprite_knight_red;
    [SerializeField] public Sprite sprite_priest_blue;
    [SerializeField] public Sprite sprite_priest_red;
    [SerializeField] public Sprite sprite_soldier_blue;
    [SerializeField] public Sprite sprite_soldier_red;
    
    // Spells
    [SerializeField] private GameObject anim_spell_smite;
    [SerializeField] private GameObject anim_spell_snipe;
    [SerializeField] private GameObject anim_spell_heavenlySmite;
    [SerializeField] private GameObject anim_spell_prayer;
    [SerializeField] private GameObject anim_spell_vitality;
    [SerializeField] private GameObject anim_spell_endurance;
    [SerializeField] private GameObject anim_spell_vigor;
    [SerializeField] private GameObject anim_spell_nimbleness;
    [SerializeField] private GameObject anim_spell_agility;
    [SerializeField] private GameObject anim_spell_precision;
    [SerializeField] private GameObject anim_spell_oracle;
    [SerializeField] private GameObject anim_spell_disarmTrap;
    [SerializeField] private GameObject anim_spell_provisions;
    [SerializeField] private GameObject anim_spell_reinforcements;
    [SerializeField] private GameObject anim_spell_greed;
    [SerializeField] private GameObject anim_spell_warcry;
    [SerializeField] private GameObject anim_spell_rebirth;
    [SerializeField] private GameObject anim_spell_assassinate;
    [SerializeField] private GameObject anim_spell_bearTrap;
    [SerializeField] private GameObject anim_spell_landMine;
    [SerializeField] private GameObject anim_spell_royalPledge;
    
    [SerializeField] private GameObject anim_unit_soldier;
    [SerializeField] private GameObject anim_unit_knight;
    [SerializeField] private GameObject anim_unit_assassin;
    [SerializeField] private GameObject anim_unit_priest;
    [SerializeField] private GameObject anim_unit_archer;
    [SerializeField] private GameObject anim_unit_dragonRider;

    #endregion


    private bool placerClicked = false;
    private const float lockAxis = 27f;

    private HashSet<Node> validMove;
    private Pathfinding pf;
    private Unit currentUnit;
    private Unit[] unitArray;
    private float timeInterval = 0;

    private Grid grid;

    void Awake()
    {
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        grid = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();
    }
    
    public void CreateUnit(Unit.UnitTypes unit, Node positionNode)
    {
        Unit placedUnit = null;

        int playerId = GameLoop.instance.GetCurrentPlayer().PlayerId;

        if (unit == Unit.UnitTypes.Soldier)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/Soldier", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_soldier", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_soldier.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_soldier, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned a Soldier at (" + positionNode.gridX + ", " +  positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned a Soldier!", TurnManager.instance.color32_green);
            
            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_soldier_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_soldier_red;
            }
        }
        else if (unit == Unit.UnitTypes.Knight)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/Knight", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_knight", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_knight.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_knight, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned a Knight at (" + positionNode.gridX + ", " + positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned a Knight!", TurnManager.instance.color32_green);
            
            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_knight_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_knight_red;
            }
        }
        else if (unit == Unit.UnitTypes.Assassin)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/Assassin", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_assassin", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_assassin.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_assassin, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned an Assassin at (" + positionNode.gridX + ", " + positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned an Assassin!", TurnManager.instance.color32_green);
            
            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_assassin_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_assassin_red;
            }
        }
        else if (unit == Unit.UnitTypes.Priest)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/Priest", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_priest", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_priest.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_priest, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned a Priest at (" + positionNode.gridX + ", " + positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned an Priest!", TurnManager.instance.color32_green);
            
            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_priest_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_priest_red;
            }
        }
        else if (unit == Unit.UnitTypes.Archer)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/Archer", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_archer", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_archer.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_archer, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned an Archer at (" + positionNode.gridX + ", " + positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned an Archer!", TurnManager.instance.color32_green);
            Instantiate(anim_unit_archer, positionNode.worldPosition, Quaternion.identity);

            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_archer_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_archer_red;
            }
            
        }  
        else if (unit == Unit.UnitTypes.DragonRider)
        {
            if (GameManager.instance.networked)
            {
                placedUnit = PhotonNetwork.Instantiate("Units/DragonRider", positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                PhotonNetwork.Instantiate("UnitAnimation/unit_dragonRider", positionNode.worldPosition, Quaternion.identity);
            }
            else
            {
                placedUnit = Instantiate(m_dragonRider.gameObject, positionNode.worldPosition, Quaternion.identity).GetComponent<Unit>();
                Instantiate(anim_unit_dragonRider, positionNode.worldPosition, Quaternion.identity);
            }
            TurnManager.instance.updateGameHistory("Player " + playerId + " summoned a Dragon Rider at (" + positionNode.gridX + ", " + positionNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Succesfully summoned a Dragon Rider!", TurnManager.instance.color32_green);
            
            if (playerId == 0)
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_dragonRider_blue;
            }
            else
            {
                placedUnit.transform.GetComponentInChildren<SpriteRenderer>().sprite = sprite_dragonRider_red;
            }
        }

        // TODO everything below is only done on the player spawning the unit,
        // WARNING is probably breaking the use of spells after someone moves
            
        placedUnit.SetUnitPlayerID(playerId);

        positionNode.AddUnit(placedUnit);

        //hardcoded color for test
        /*
        if (playerId == 0)
            placedUnit.transform.GetComponent<Renderer>().material.color = Color.blue;
        else
            placedUnit.transform.GetComponent<Renderer>().material.color = Color.red;
        */
        
        GameLoop.instance.GetCurrentPlayer().AddUnit(placedUnit);

        placedUnit.CheckHostileTrapOrItemInNode(positionNode);

        TurnManager.instance.cardSuccessful = true;
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().DecreaseCurrentHealthBy(5); // smite damage
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Smite on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Smite on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_smite, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().DecreaseCurrentHealthBy(10); // smite damage
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Snipe on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") lost 10 health!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Snipe on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_snipe, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 1);

        string spellMessage = "";

        spellMessage += "Player " + playerId + " used Heavenly Smite at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

        foreach (Node node in allNodesInRange)
        {
            if(node.GetUnit() != null)
            {
                if(node.GetUnit().GetUnitPlayerID() != playerId)
                {
                    node.GetUnit().DecreaseCurrentHealthBy(3);
                    spellMessage += node.GetUnit().GetUnitType() + " (" + node.gridX + "," + node.gridY + ") lost 3 health!\n";
                }
            }
        }

        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Heavenly Smite at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
        Instantiate(anim_spell_heavenlySmite, selectedNode.GetUnit().transform, false);
        TurnManager.instance.cardSuccessful = true;

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
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 1);

        string spellMessage = "";

        spellMessage += "Player " + playerId + " used Prayer at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                {
                    spellMessage += node.GetUnit().GetUnitType() + " (" + node.gridX + "," + node.gridY + ") gained 3 health!\n";
                    node.GetUnit().IncreaseCurrentHealthBy(3);
                }
            }
        }
        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Prayer at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
        TurnManager.instance.cardSuccessful = true;
        Instantiate(anim_spell_prayer, selectedNode.GetUnit().transform, false);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseMaxHealthBy(5);
            selectedNode.GetUnit().IncreaseCurrentHealthBy(5);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Vitality on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 5 maximum health!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Vitality on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_vitality, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseDefenceBy(2);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Endurance on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 2 defence!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Endurance on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_endurance, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseDamageBy(3);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Vigor on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 5 damage!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Vigor on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_vigor, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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

        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseMovementSpeedBy(1);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Nimbleness on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 1 movement speed!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Nimbleness on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_nimbleness, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseEvasionBy(10);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Agility on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 10 evasion!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Agility on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_agility, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseAccuracyBy(10);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Precision on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 10 accuracy!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Precision on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_precision, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 0, 2);

        string spellMessage = "Player " + playerId + " used Oracle at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

        foreach (Node node in allNodesInRange)
        {
            if(node.GetTrapOrItemList().Count > 0)
            {
                foreach(TrapOrItem toi in node.GetTrapOrItemList())
                {
                    if (toi.GetTrapOrItemPlayerID() != playerId)
                        spellMessage += "Enemy trap found at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";
                }
            }
        }

        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Oracle at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
        TurnManager.instance.cardSuccessful = true;
        Instantiate(anim_spell_oracle, selectedNode.GetUnit().transform, false);
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
        string spellMessage = "Player " + playerId + " used Disarm Trap at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";


        if (selectedNode.GetTrapOrItemList().Count > 0)
        {
            foreach (TrapOrItem toi in selectedNode.GetTrapOrItemList())
            {
                if (toi.GetTrapOrItemPlayerID() != playerId)
                {
                    selectedNode.RemoveTrapOrItem(toi);
                    spellMessage += "Enemy trap removed at (" + selectedNode.gridX + ", " + selectedNode.gridY + ")!\n";
                    Instantiate(anim_spell_disarmTrap, selectedNode.GetUnit().transform, false);
                }
            }
        }

        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Disarm Trap at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            selectedNode.GetUnit().IncreaseCurrentHealthBy(5);
            TurnManager.instance.updateGameHistory("Player " + playerId + " used Provisions on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") gained 5 health!\n");
            TurnManager.instance.updateTurnUpdate("Successfully used Provisions on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            Instantiate(anim_spell_provisions, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
            string spellMessage = "Player " + playerId + " used Reinforcements at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

            foreach (Node node in allNodesInRange)
            {
                CreateUnit(Unit.UnitTypes.Soldier, node);
                spellMessage += "Player " + playerId + " summoned a Soldier at (" + node.gridX + "," + node.gridY + ")!\n";
            }

            TurnManager.instance.updateGameHistory(spellMessage);
            TurnManager.instance.updateTurnUpdate("Successfully used Reinforcements on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            // Instantiate(anim_spell_reinforcements, selectedNode.GetUnit().transform, false);
        }
        else
            TurnManager.instance.updateTurnUpdate("Cannot use Reinforcements on this unit! Its surrounding cells must be empty.", TurnManager.instance.color32_red);

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
        GameLoop.instance.GetCurrentPlayer().AddCard(TurnManager.instance.RandomCard());
        GameLoop.instance.GetCurrentPlayer().AddCard(TurnManager.instance.RandomCard());
        TurnManager.instance.updateGameHistory("Player " + playerId + " used Greed!\nTwo random cards added to their hand!\n");
        TurnManager.instance.updateTurnUpdate("Successfully used Greed!", TurnManager.instance.color32_green);
        TurnManager.instance.cardSuccessful = true;
        // Will only use sound
        Instantiate(anim_spell_greed);
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

        string spellMessage = "Player " + playerId + " used Warcry on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                {
                    selectedNode.GetUnit().IncreaseDamageBy(2);
                    spellMessage += node.GetUnit().GetUnitType() + " (" + node.gridX + "," + node.gridY + ") gained 2 damage!\n";
                }
            }
        }
        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Warcry on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
        TurnManager.instance.cardSuccessful = true;
        Instantiate(anim_spell_warcry, selectedNode.GetUnit().transform, false);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            if (selectedNode.GetUnit().GetCurrentHealth() != selectedNode.GetUnit().GetMaxHealth())
            {
                selectedNode.GetUnit().SetCurrentHealth(selectedNode.GetUnit().GetMaxHealth());
                TurnManager.instance.updateGameHistory("Player " + playerId + " used Rebirth on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") was restored to full health!\n");
                TurnManager.instance.updateTurnUpdate("Successfully used Rebirth on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
                TurnManager.instance.cardSuccessful = true;
                Instantiate(anim_spell_rebirth, selectedNode.GetUnit().transform, false);
            }
            else
                TurnManager.instance.updateTurnUpdate("This target is already full health!", TurnManager.instance.color32_red);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
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
        if (selectedNode.GetUnit() != null) // check if there's a unit on this node
        {
            if (selectedNode.GetUnit().GetUnitType() != Unit.UnitTypes.King) // confirm that the unit is not ours
            {
                TurnManager.instance.updateGameHistory("Player " + playerId + " used Assassinate on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n" + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") was slain!\n");
                TurnManager.instance.updateTurnUpdate("Successfully used Assassinate on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
                selectedNode.GetUnit().SetCurrentHealth(0); // smite damage
                TurnManager.instance.cardSuccessful = true;
                DisplayAnimation(anim_spell_assassinate);
            }
            else
                TurnManager.instance.updateTurnUpdate("Assassinate cannot be used on a King!", TurnManager.instance.color32_red);
        }
        else
            TurnManager.instance.updateTurnUpdate("There is no target on this cell!", TurnManager.instance.color32_red);
    }


    /*
     * Card ID: 24
     * Card ID: 
     * Card Name: Bear Trap
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages the triggering unit for 5 health.
     * Cost: 3
     */
    public void spell_bearTrap(int playerId, Node selectedNode)
    {
        if (selectedNode.GetUnit() == null) // check if there's a unit on this node
        {
            TrapOrItem bearTrap = new TrapOrItem(playerId, 0, 0, 0, TrapOrItem.TrapOrItemTypes.BearTrap);
            selectedNode.AddTrapOrItem(bearTrap);
            TurnManager.instance.updateGameHistory("Player " + playerId + " placed a Bear Trap on the battlefield!\n");
            TurnManager.instance.updateTurnUpdate("Successfully placed Bear Trap at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;

            DisplayAnimation(anim_spell_bearTrap);

        }
        else
            TurnManager.instance.updateTurnUpdate("Cannot place a Bear on a cell that has a unit on it!", TurnManager.instance.color32_red);
    }

    // for displaying animations without a unit on position to instantiate
    public void DisplayAnimation(GameObject anim)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 vec = new Vector3(hit.transform.position.x, 1, hit.transform.position.z);
            Instantiate(anim, vec, Quaternion.identity);
        }
    }

    /*
     * Card ID: 25
     * Card Name: Land Mine
     * Type: Spell(Trap)
     * Cast Range: (1, 2) tiles from any friendly unit
     * Effect: Damages all units from (0,1) tiles from the detonation origin for 3 health.
     * Cost: 3
     */
    public void spell_landMine(int playerId, Node selectedNode)
    {
        if (selectedNode.GetUnit() == null) // check if there's a unit on this node
        {
            TrapOrItem landMine = new TrapOrItem(playerId, 0, 0, 0, TrapOrItem.TrapOrItemTypes.LandMine);
            selectedNode.AddTrapOrItem(landMine);
            TurnManager.instance.updateGameHistory("Player " + playerId + " placed a Land Mine on the battlefield!\n");
            TurnManager.instance.updateTurnUpdate("Successfully placed a Land Mine at (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
            TurnManager.instance.cardSuccessful = true;
            DisplayAnimation(anim_spell_landMine);
        }
        else
            TurnManager.instance.updateTurnUpdate("Cannot place a Bear on a cell that has a unit on it!", TurnManager.instance.color32_red);
    }

    /*
     * Card ID: 26
     * Card Name: Royal Pledge
     * Type: Spell(Active)
     * Cast Range: (0, 0) tiles from a friendly King
     * Effect: Increases the damage, current health and maximum health of all allies within (1,1) tiles of the casting origin by 2 
     * Cost: 4
     */
    public void spell_royalPledge(int playerId, Node selectedNode, Vector3 selectedNodePosition)
    {
        HashSet<Node> allNodesInRange = pf.GetNodesMinMaxRange(selectedNodePosition, false, 1, 1);

        string spellMessage = "Player " + playerId + " used Royal Pledge on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n";

        foreach (Node node in allNodesInRange)
        {
            if (node.GetUnit() != null)
            {
                if (node.GetUnit().GetUnitPlayerID() == playerId)
                {
                    node.GetUnit().IncreaseDamageBy(2);
                    node.GetUnit().IncreaseMaxHealthBy(2);
                    node.GetUnit().IncreaseCurrentHealthBy(2);
                    spellMessage += node.GetUnit().GetUnitType() + " (" + node.gridX + ", " + node.gridY + ") gained 2 damage, maximum health and health!\n";
                    Instantiate(anim_spell_royalPledge, selectedNode.GetUnit().transform, false);
                }
            }
        }

        TurnManager.instance.updateGameHistory(spellMessage);
        TurnManager.instance.updateTurnUpdate("Successfully used Royal Pledge on " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", TurnManager.instance.color32_green);
        TurnManager.instance.cardSuccessful = true;
    }
}

