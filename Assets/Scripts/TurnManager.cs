using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;
using UnityEngine.UI;

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] public Material availableMaterial;
    [SerializeField] public Material defaultMaterial;
    [SerializeField] public Material targetMaterial;
    [SerializeField] public Material AOEMaterial;

    private const float lockAxis = 27f;

    public Player currentPlayer = default;
    public Player localPlayer = default;

    public HashSet<Node> selectableNodes;

    private Pathfinding pf;
    private Grid grid;

    public Unit currentUnit;
    public Vector3 currentUnitPosition;

    public bool cardSelected;

    public CardEffectManager cardEffectManager;

    public Card storedCard;

    public bool cardSuccessful;

    public List<Unit> allUnits;

    public enum TurnState
    {
        DrawingCard,
        Waiting,
        Free,
        SelectingCardOrigin,
        SelectingTileMovement,
        SelectingTileAttack,
        PlacingEnemyUnit // temp for testing
    }

    public bool placingEnemyUnit; // temp for testing

    [SerializeField] public TurnState currentTurnState;

    public bool unitSelected;

    // Stat panel

    public Text unitPlayerIDText;
    public Text unitDamageText;
    public Text unitDefenceText;
    public Text unitARText;
    public Text unitMSText;
    public Text unitAccuracyText;
    public Text unitEvasionText;
    public Text unitHealthText;

    // Option Menu
    public GameObject optionPanel;

    // Card Draw

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public GameObject cardDrawPanel;

    [SerializeField] private GameObject handPanel;

    void Start()
    {
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        grid = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();

        currentTurnState = TurnState.Free;

        selectableNodes = new HashSet<Node>();

        cardSelected = false;
        cardSuccessful = false;
        placingEnemyUnit = false;
        currentPlayer = GameLoop.instance.GetCurrentPlayer();
        localPlayer = null;

        foreach(Player player in GameLoop.instance.GetPlayerList())
        {
            if (player is LocalPlayer)
                localPlayer = player;
        }

        loadPlayerHand();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTurnState == TurnState.DrawingCard)
        {
            // do nothing
        }
        else if (currentTurnState == TurnState.Free)
        {
            if (cardSelected)
                PlaceCard();
            else if (placingEnemyUnit)
                currentTurnState = TurnState.PlacingEnemyUnit;
            else if (Input.GetMouseButtonDown(0))
                checkBoardClickForUnit();
        }
        else if (currentTurnState == TurnState.SelectingTileMovement)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickMove();
        }
        else if (currentTurnState == TurnState.SelectingTileAttack)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickAttack();
        }
        else if (currentTurnState == TurnState.SelectingCardOrigin)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickCard();
        }
        else if (currentTurnState == TurnState.PlacingEnemyUnit)
        {
            if (Input.GetMouseButtonDown(0))
                placeEnemyUnit();
        }
    }

    public void loadPlayerHand()
    {
        if(localPlayer.GetHand().Count <= 5)
        {
            handPanel.transform.GetChild(0).gameObject.SetActive(true);
            handPanel.transform.GetChild(1).gameObject.SetActive(false);

            foreach (Transform child in handPanel.transform.GetChild(0).gameObject.transform)
            {
                if(child.gameObject.transform.childCount > 0)
                    GameObject.Destroy(child.gameObject.transform.GetChild(0).gameObject);
            }

            for (int x = 0; x < localPlayer.GetHand().Count; x++)
            {
                Card card = localPlayer.GetCard(x);
                GameObject cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);


                if (card is UnitCard)
                {
                    cardGO.AddComponent(typeof(UnitCard));
                    cardGO.GetComponent<UnitCard>().copyUnitCard((UnitCard)card);
                }
                else if (card is SpellCard)
                {
                    cardGO.AddComponent(typeof(SpellCard));
                    cardGO.GetComponent<SpellCard>().copySpellCard((SpellCard)card);
                }
                cardGO.transform.SetParent(handPanel.transform.GetChild(0).transform.GetChild(x));
                cardGO.AddComponent(typeof(CardUI));
                cardGO.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                cardGO.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                cardGO.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                cardGO.GetComponent<Button>().onClick.AddListener(PlaceCard);
            }
        }
        else if(localPlayer.GetHand().Count > 5)
        {
            handPanel.transform.GetChild(0).gameObject.SetActive(false);
            handPanel.transform.GetChild(1).gameObject.SetActive(true);

            GameplayUIManager.instance.handCount = localPlayer.GetHand().Count;
            GameplayUIManager.instance.fillDynamicHand();

            for (int i = 0; i < localPlayer.GetHand().Count; i++)
            {
                Card card = localPlayer.GetCard(i);
                GameObject cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                if (card is UnitCard)
                {
                    cardGO.AddComponent(typeof(UnitCard));
                    cardGO.GetComponent<UnitCard>().copyUnitCard((UnitCard)card);
                }
                else if (card is SpellCard)
                {
                    cardGO.AddComponent(typeof(SpellCard));
                    cardGO.GetComponent<SpellCard>().copySpellCard((SpellCard)card);
                }
   
                cardGO.transform.SetParent(handPanel.transform.GetChild(1).transform.GetChild(i));

                cardGO.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                cardGO.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                cardGO.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

                cardGO.AddComponent(typeof(CardUI));

                cardGO.GetComponent<Button>().onClick.AddListener(PlaceCard);
            }
        }
    }

    // temp function to place enemy units
    void placeEnemyUnit()
    {
        Node selectedNode = null;
        Vector3 selectedNodePosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Tile"))
            {
                selectedNode = grid.NodeFromWorldPoint(hit.transform.position);
                selectedNodePosition = hit.transform.position;
                break;
            }
        }

        if (selectedNode != null)
        {
            if (selectedNode.unitInThisNode == null)
            {
                cardEffectManager.createSoldierUnit(1, selectedNode); // assuming enemy player id = 1
            }
        }
        currentTurnState = TurnState.Free;
        placingEnemyUnit = false;
    }

    public void unitMove()
    {
        if (currentUnit != null && currentUnit.GetUnitPlayerID() == currentPlayer.PlayerId)
        {
            // Display All Valid Tiles to move on
            int unitMoveSpeed = (int)currentUnit.GetMovementSpeed();
            Node currentNode = grid.NodeFromWorldPoint(currentUnitPosition);

            pf.minDepthLimit = 1;
            pf.maxDepthLimit = unitMoveSpeed;
            selectableNodes = pf.GetNodesMinMaxRange(currentUnitPosition, false, 1, unitMoveSpeed);

            if (selectableNodes != null && selectableNodes.Count > 0)
            {
                foreach (var node in selectableNodes)
                {
                    //jon hover script
                    Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>());

                    //rey hover script
                    Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(TileOnMouseOver));
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = availableMaterial;
                }
            }

            // set the units' layer to ignore raycast (used because the unit model stopped raycast from hitting tile, to show AOE)
            List<Node> allUnitsNodes = grid.GetAllUnitNodes();

            foreach (Node node in allUnitsNodes)
                node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            currentTurnState = TurnState.SelectingTileMovement;
        }
    }

    public void unitAttack()
    {
        if (currentUnit != null && currentUnit.GetUnitPlayerID() == currentPlayer.PlayerId)
        {
            Node currentNode = grid.NodeFromWorldPoint(currentUnitPosition);

            pf.minDepthLimit = currentUnit.GetMinRange();
            pf.maxDepthLimit = currentUnit.GetMaxRange();
            selectableNodes = pf.GetNodesMinMaxRange(currentUnitPosition, false, currentUnit.GetMinRange(), currentUnit.GetMaxRange());

            if (selectableNodes != null && selectableNodes.Count > 0)
            {
                foreach (var node in selectableNodes)
                {
                    //jon hover script
                    Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>());

                    Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(TileOnMouseOver));
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = targetMaterial;
                }
            }

            // set the units' layer to ignore raycast (used because the unit model stopped raycast from hitting tile, to show AOE)
            List<Node> allUnitsNodes = grid.GetAllUnitNodes();

            foreach (Node node in allUnitsNodes)
                node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            currentTurnState = TurnState.SelectingTileAttack;
        }
    }


    void checkBoardClickForUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // shoot raycast towards board
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Unit")) // if board hits unit
            {
                if (hit.transform.GetComponent<Unit>().GetUnitPlayerID() == currentPlayer.PlayerId) // check if unit belongs to current player
                {
                    currentUnit = hit.transform.GetComponent<Unit>();
                    currentUnitPosition = hit.transform.position;

                    optionPanel.SetActive(true);
                }
                else
                    optionPanel.SetActive(false);

                unitPlayerIDText.text = "" + hit.transform.GetComponent<Unit>().GetUnitPlayerID();
                unitDamageText.text = "" + hit.transform.GetComponent<Unit>().GetDamage();
                unitDefenceText.text = "" + hit.transform.GetComponent<Unit>().GetDefence();
                unitARText.text = "" + hit.transform.GetComponent<Unit>().GetMinRange() + " - " +  + hit.transform.GetComponent<Unit>().GetMaxRange();
                unitMSText.text = "" + hit.transform.GetComponent<Unit>().GetMovementSpeed();
                unitAccuracyText.text = "" + hit.transform.GetComponent<Unit>().GetAccuracy();
                unitEvasionText.text = "" + hit.transform.GetComponent<Unit>().GetEvasion();
                unitHealthText.text = "" + hit.transform.GetComponent<Unit>().GetCurrentHealth() + "/" + + hit.transform.GetComponent<Unit>().GetMaxHealth();
            }
        }
    }

    void validateSelectTileClickMove()
    {
        Node selectedNode = null;
        Vector3 selectedNodePosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Tile"))
            {
                selectedNode = grid.NodeFromWorldPoint(hit.transform.position);
                selectedNodePosition = hit.transform.position;
                break;
            }
        }

        if (selectedNode != null)
        {
            if (selectableNodes.Contains(selectedNode) && selectedNode.unitInThisNode == null)
            {
                currentUnit.isClicked = true;
                currentUnitPosition = selectedNodePosition;
                currentUnit.SelectNewUnitPosition();
            }
        }

        foreach (var node in selectableNodes)
        {
            Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<TileOnMouseOver>());

            //Jon hover script
            Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(Hover));
            Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>().hoveredTile = AOEMaterial;

        }

        // set the units back to their original layer
        List<Node> allUnitsNodes = grid.GetAllUnitNodes();

        foreach (Node node in allUnitsNodes)
            node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Unit");

        ResetMaterial();
        selectableNodes.Clear();
        unselectUnit();
        currentTurnState = TurnState.Free;
    }

    void validateSelectTileClickAttack()
    {
        Node selectedNode = null;
        Vector3 selectedNodePosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Tile"))
            {
                selectedNode = grid.NodeFromWorldPoint(hit.transform.position);
                selectedNodePosition = hit.transform.position;
                break;
            }
        }

        if (selectedNode != null)
        {
            if (selectedNode.unitInThisNode != null && selectedNode.unitInThisNode.GetUnitPlayerID() != currentPlayer.PlayerId)
                Attack(currentUnit, selectedNode.unitInThisNode);
        }

        foreach (var node in selectableNodes)
        {
            Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<TileOnMouseOver>());

            //Jon hover script
            Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(Hover));
            Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>().hoveredTile = AOEMaterial;
        }

        // set the units back to their original layer
        List<Node> allUnitsNodes = grid.GetAllUnitNodes();

        foreach (Node node in allUnitsNodes)
            node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Unit");

        ResetMaterial();
        selectableNodes.Clear();
        unselectUnit();
        currentTurnState = TurnState.Free;
    }

    void Attack(Unit attacker, Unit receiver)
    {
        int damageDealt = Mathf.Max(0, attacker.GetDamage() - receiver.GetDefence());

        int hitChance = Mathf.Max(0, (int)Mathf.Floor(attacker.GetAccuracy() - receiver.GetEvasion() / 2));
        int roll = Random.Range(0, 101); // generate 0-100

        if(roll <= hitChance)
        {
            receiver.SetCurrentHealth(receiver.GetCurrentHealth() - damageDealt);
            print("Attack successful!");
        }
        else
            print("Attack missed!");
    }

    private void ResetMaterial()
    {
        if (selectableNodes != null)
        {
            foreach (var node in selectableNodes)
            {
                Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                newMat.material = defaultMaterial;
            }
        }
    }

    public void PlaceCard()
    {
        // create temp card
        /*
        Card unitCard_soldier = new UnitCard(0, CardType.Unit, CastType.OnEmpty, "Unit: Soldier", GameObject cardImage, 1, 1, 2, 10, 5, 1, 1, 1, 4, 80, 20, false);
        Card unitCard_knight = new UnitCard(1, CardType.Unit, CastType.OnEmpty, "Unit: Knight", GameObject cardImage, 3, 1, 2, 30, 7, 4, 1, 1, 3, 70, 10, false);
        Card unitCard_assassin = new UnitCard(2, CardType.Unit, CastType.OnEmpty, "Unit: Assassin", GameObject cardImage, 3, 1, 2, 15, 9, 0, 1, 1, 8, 95, 60, false);
        Card unitCard_priest = new UnitCard(3, CardType.Unit, CastType.OnEmpty, "Unit: Priest", GameObject cardImage, 1, 1, 2, 15, 5, 0, 0, 2, 4, 100, 30, false);
        Card unitCard_archer = new UnitCard(4, CardType.Unit, CastType.OnEmpty, "Unit: Archer", GameObject cardImage, 1, 1, 2, 15, 6, 0, 2, 3, 4, 90, 30, false);
        Card unitCard_dragonRider = new UnitCard(5, CardType.Unit, CastType.OnEmpty, "Unit: DragonRider", GameObject cardImage, 1, 1, 2, 25, 6, 2, 1, 1, 6, 85, 20, true);

        Card activeCard_smite = new SpellCard(6, CardType.Active, CastType.OnEnemy, "Smite", GameObject cardImage, 1, 1, 1, "Damages an enemy unit for 5 health.");

        Card currentCard = unitCard_soldier;
        */

        GameObject clickedButtonGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject; // gets object that calls onClick
        Card currentCard = clickedButtonGO.GetComponent<Card>();
        storedCard = currentCard;

        if (grid != null)
        {
            List<Node> allyNodes = grid.GetAllyUnitNodes(currentPlayer.PlayerId); // hard code player 0
            foreach (Node node in allyNodes)
            {
                Vector3 nodePos = node.unitInThisNode.transform.position;
                selectableNodes.UnionWith(pf.GetNodesMinMaxRange(nodePos, false, currentCard.minRange, currentCard.maxRange));
            }

            if (selectableNodes != null && selectableNodes.Count > 0)
            {
                foreach (var node in selectableNodes)
                {
                    //jon hover script
                    Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>());

                    Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(TileOnMouseOver));
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = targetMaterial;
                }
            }
        }

        // set the units' layer to ignore raycast (used because the unit model stopped raycast from hitting tile, to show AOE)
        List<Node> allUnitsNodes = grid.GetAllUnitNodes();

        foreach (Node node in allUnitsNodes)
            node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        currentTurnState = TurnState.SelectingCardOrigin;
    }

    void validateSelectTileClickCard()
    {
        Node selectedNode = null;
        Vector3 selectedNodePosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform.CompareTag("Tile"))
            {
                selectedNode = grid.NodeFromWorldPoint(hit.transform.position);
                selectedNodePosition = hit.transform.position;
                break;
            }
        }

        if (selectableNodes.Contains(selectedNode))
        {
            if (storedCard.castType == CastType.OnAlly)
            {
                if (selectedNode.GetUnit() != null)
                {
                    if (selectedNode.GetUnit().GetUnitPlayerID() == currentPlayer.PlayerId)
                    {
                        if (storedCard.id == 10)
                            cardEffectManager.spell_vitality(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 11)
                            cardEffectManager.spell_endurance(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 12)
                            cardEffectManager.spell_vigor(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 13)
                            cardEffectManager.spell_nimbleness(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 14)
                            cardEffectManager.spell_agility(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 15)
                            cardEffectManager.spell_precision(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 18)
                            cardEffectManager.spell_provisions(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 19)
                            cardEffectManager.spell_reinforcements(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                        else if (storedCard.id == 21)
                            cardEffectManager.spell_warcry(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                        else if (storedCard.id == 22)
                            cardEffectManager.spell_rebirth(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 24)
                            cardEffectManager.spell_teleport(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 27)
                            cardEffectManager.spell_royalPledge(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                    }
                }
            }
            else if (storedCard.castType == CastType.OnEnemy)
            { 
                if (selectedNode.GetUnit() != null)
                {
                    if (selectedNode.GetUnit().GetUnitPlayerID() != currentPlayer.PlayerId)
                    {
                        if (storedCard.id == 6)
                            cardEffectManager.spell_smite(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 7)
                            cardEffectManager.spell_snipe(currentPlayer.PlayerId, selectedNode);
                        else if (storedCard.id == 23)
                            cardEffectManager.spell_assassinate(currentPlayer.PlayerId, selectedNode);
                    }
                }
            }
            else if (storedCard.castType == CastType.OnEmpty)
            {
                if (selectedNode.unitInThisNode == null)
                {
                    if (storedCard.id == 0) // spawn soldier
                        cardEffectManager.createSoldierUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 1) // spawn Knight
                        cardEffectManager.createKnightUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 2) // spawn Assassin
                        cardEffectManager.createAssassinUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 3) // spawn Priest
                        cardEffectManager.createPriestUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 4) // spawn Archer
                        cardEffectManager.createArcherUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 5) // spawn Dragon Rider
                        cardEffectManager.createDragonRiderUnit(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 16)
                        cardEffectManager.spell_oracle(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                    else if (storedCard.id == 17)
                        cardEffectManager.spell_disarmTrap(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                    else if (storedCard.id == 25)
                        cardEffectManager.spell_bearTrap(currentPlayer.PlayerId, selectedNode);
                    else if (storedCard.id == 26)
                        cardEffectManager.spell_landMine(currentPlayer.PlayerId, selectedNode);
                }
            }
            else if (storedCard.castType == CastType.OnAny)
            {
                if (storedCard.id == 8)
                    cardEffectManager.spell_heavenlySmite(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                else if (storedCard.id == 9)
                    cardEffectManager.spell_prayer(currentPlayer.PlayerId, selectedNode, selectedNodePosition);
                else if (storedCard.id == 20)
                    cardEffectManager.spell_greed(currentPlayer.PlayerId);
            }
        }

        foreach (var node in selectableNodes)
        {
            Destroy(Grid.tileTrack[node.gridX, node.gridY].GetComponent<TileOnMouseOver>());

            //Jon hover script
            Grid.tileTrack[node.gridX, node.gridY].AddComponent(typeof(Hover));
            Grid.tileTrack[node.gridX, node.gridY].GetComponent<Hover>().hoveredTile = AOEMaterial;
        }

        pf.minDepthLimit = storedCard.aoeMinRange;
        pf.maxDepthLimit = storedCard.aoeMaxRange;

        HashSet<Node> aoeNodes = pf.GetNodesMinMaxRange(selectedNodePosition, false, storedCard.aoeMinRange, storedCard.aoeMaxRange);
        foreach (Node node in aoeNodes)
        {
            Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
            newMat.material = defaultMaterial;
        }
        
        // set the units back to their original layer
        List<Node> allUnitsNodes = grid.GetAllUnitNodes();

        foreach (Node node in allUnitsNodes)
            node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Unit");

        if(cardSuccessful)
            currentPlayer.RemoveCard(storedCard);

        cardSuccessful = false;

        loadPlayerHand();
        ResetMaterial();
        selectableNodes.Clear();
        unselectUnit();
        currentTurnState = TurnState.Free;
        cardSelected = false;
    }

    public void unselectUnit()
    {
        currentUnit = null;
        currentUnitPosition = Vector3.zero;

        unitPlayerIDText.text = "-";
        unitDamageText.text = "-";
        unitDefenceText.text = "-";
        unitARText.text = "x - x";
        unitMSText.text = "-";
        unitAccuracyText.text = "-";
        unitEvasionText.text = "-";
        unitHealthText.text = "-/-";

        optionPanel.SetActive(false);
    }

    public void ShowCardSelection()
    {
        cardDrawPanel.SetActive(true);

        UnitCard unitCard;
        SpellCard spellCard;
        GameObject cardGO;

        for (int x = 0; x < 5; x++)
        {
            if(cardDrawPanel.transform.GetChild(0).transform.GetChild(x).childCount > 0)
                Destroy(cardDrawPanel.transform.GetChild(0).transform.GetChild(x).transform.GetChild(0).gameObject);

            RectTransform rt = cardDrawPanel.transform.GetChild(0).transform.GetChild(x).GetComponent<RectTransform>();

            /*Top*/
            rt.offsetMax = new Vector2(0, 0);
            /*Bottom*/
            rt.offsetMin = new Vector2(0, 0);

            int random = Random.Range(0, 28);

            switch (random)
            {
                case 0:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 0;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Soldier";
                    unitCard.cost = 1;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 10;
                    unitCard.damage = 5;
                    unitCard.defence = 1;
                    unitCard.minAttackRange = 1;
                    unitCard.maxAttackRange = 1;
                    unitCard.moveSpeed = 4;
                    unitCard.accuracy = 80;
                    unitCard.evasion = 20;
                    unitCard.flying = false;

                    break;

                case 1:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 1;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Knight";
                    unitCard.cost = 3;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 20;
                    unitCard.damage = 7;
                    unitCard.defence = 3;
                    unitCard.minAttackRange = 1;
                    unitCard.maxAttackRange = 1;
                    unitCard.moveSpeed = 3;
                    unitCard.accuracy = 70;
                    unitCard.evasion = 10;
                    unitCard.flying = false;

                    break;

                case 2:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 2;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Assassin";
                    unitCard.cost = 3;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 15;
                    unitCard.damage = 9;
                    unitCard.defence = 0;
                    unitCard.minAttackRange = 1;
                    unitCard.maxAttackRange = 1;
                    unitCard.moveSpeed = 6;
                    unitCard.accuracy = 95;
                    unitCard.evasion = 60;
                    unitCard.flying = false;

                    break;

                case 3:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 3;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Priest";
                    unitCard.cost = 3;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 15;
                    unitCard.damage = 5;
                    unitCard.defence = 0;
                    unitCard.minAttackRange = 0;
                    unitCard.maxAttackRange = 2;
                    unitCard.moveSpeed = 4;
                    unitCard.accuracy = 100;
                    unitCard.evasion = 30;
                    unitCard.flying = false;

                    break;

                case 4:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 4;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Archer";
                    unitCard.cost = 3;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 15;
                    unitCard.damage = 6;
                    unitCard.defence = 0;
                    unitCard.minAttackRange = 2;
                    unitCard.maxAttackRange = 3;
                    unitCard.moveSpeed = 4;
                    unitCard.accuracy = 90;
                    unitCard.evasion = 30;
                    unitCard.flying = false;

                    break;

                case 5:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = 5;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "Dragon Rider";
                    unitCard.cost = 5;
                    unitCard.minRange = 1;
                    unitCard.maxRange = 2;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 25;
                    unitCard.damage = 6;
                    unitCard.defence = 2;
                    unitCard.minAttackRange = 1;
                    unitCard.maxAttackRange = 1;
                    unitCard.moveSpeed = 4;
                    unitCard.accuracy = 85;
                    unitCard.evasion = 20;
                    unitCard.flying = false;

                    break;

                case 6:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 6;
                    spellCard.castType = CastType.OnEnemy;
                    spellCard.name = "Smite";
                    spellCard.cost = 2;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 1;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Damages an enemy unit for 5 health.";

                    break;

                case 7:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 7;
                    spellCard.castType = CastType.OnEnemy;
                    spellCard.name = "Snipe";
                    spellCard.cost = 3;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 3;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Only castable from a friendly Archer unit. Damages an enemy unit for 10 health.";

                    break;

                case 8:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 8;
                    spellCard.castType = CastType.OnAny;
                    spellCard.name = "Heavenly Smite";
                    spellCard.cost = 5;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 3;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 1;
                    spellCard.description = "Damages all enemies within (0,1) tiles of the casting origin for 3 health.";

                    break;

                case 9:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 9;
                    spellCard.castType = CastType.OnAny;
                    spellCard.name = "Prayer";
                    spellCard.cost = 4;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 2;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 1;
                    spellCard.description = "Only castable from a friendly Priest unit. Heal all allies within (0,1) tiles of the casting origin for 3 health.";

                    break;

                case 10:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 10;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Vitality";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s current and maximum Health by 5.";

                    break;

                case 11:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 11;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Endurance";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s Defence by 2.";

                    break;

                case 12:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 12;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Vigor";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s Damage by 3.";

                    break;

                case 13:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 13;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Nimbleness";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s movement speed by 1.";

                    break;

                case 14:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 14;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Agility";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s Evasion by 10.";

                    break;

                case 15:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 15;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Precision";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Increases an ally unit�s Accuracy by 10.";

                    break;

                case 16:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 16;
                    spellCard.castType = CastType.OnEmpty;
                    spellCard.name = "Oracle";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 5;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 2;
                    spellCard.description = "Reveals all traps within (0,2) tiles of the triggering origin.";

                    break;

                case 17:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 17;
                    spellCard.castType = CastType.OnEmpty;
                    spellCard.name = "Disarm Trap";
                    spellCard.cost = 1;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 3;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Attempts to disarm an enemy trap on a tile.";

                    break;

                case 18:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 18;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Provisions";
                    spellCard.cost = 1;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Heals a unit for 5 health.";

                    break;

                case 19:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 19;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Reinforcements";
                    spellCard.cost = 6;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Summons 4 Soldier units around an ally.";

                    break;

                case 20:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 20;
                    spellCard.castType = CastType.OnAny;
                    spellCard.name = "Reinforcements";
                    spellCard.cost = 3;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Draw 2 cards.";

                    break;

                case 21:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 21;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Warcry";
                    spellCard.cost = 4;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 1;
                    spellCard.description = "Only castable from a friendly Knight unit. Increases the damage of all allies within (0,1) tiles of the casting origin by 2.";

                    break;

                case 22:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 22;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Rebirth";
                    spellCard.cost = 4;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Fully heals an ally unit.";

                    break;

                case 23:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 23;
                    spellCard.castType = CastType.OnEnemy;
                    spellCard.name = "Assassinate";
                    spellCard.cost = 5;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 1;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Only castable from a friendly Assassin unit. Immediately kills and enemy unit (does not work on King unit).";

                    break;

                case 24:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 24;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Teleport";
                    spellCard.cost = 1;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 2;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Teleports a unit (1,2) tiles.";

                    break;

                case 25:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 25;
                    spellCard.castType = CastType.OnEmpty;
                    spellCard.name = "Bear Trap";
                    spellCard.cost = 2;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 2;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Damages the triggering unit for 5 health.";

                    break;

                case 26:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 26;
                    spellCard.castType = CastType.OnEmpty;
                    spellCard.name = "Land Mine";
                    spellCard.cost = 3;
                    spellCard.minRange = 1;
                    spellCard.maxRange = 2;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 1;
                    spellCard.description = "Damages all units from (0,1) tiles from the detonation origin for 3 health.";

                    break;

                case 27:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(SpellCard));

                    spellCard = cardGO.GetComponent<SpellCard>();

                    spellCard.id = 27;
                    spellCard.castType = CastType.OnAlly;
                    spellCard.name = "Royal Pledge";
                    spellCard.cost = 3;
                    spellCard.minRange = 0;
                    spellCard.maxRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.aoeMinRange = 0;
                    spellCard.description = "Only castable from a friendly King unit. Increases the damage, current health and max health of all allies within (1,1) tiles of the casting origin by 2.";

                    break;

                default:
                    cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                    cardGO.AddComponent(typeof(UnitCard));

                    unitCard = cardGO.GetComponent<UnitCard>();

                    unitCard.id = -1;
                    unitCard.castType = CastType.OnEmpty;
                    unitCard.name = "-";
                    unitCard.cost = 0;
                    unitCard.minRange = 0;
                    unitCard.maxRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.aoeMinRange = 0;
                    unitCard.health = 0;
                    unitCard.damage = 0;
                    unitCard.defence = 0;
                    unitCard.minAttackRange = 0;
                    unitCard.maxAttackRange = 0;
                    unitCard.moveSpeed = 0;
                    unitCard.accuracy = 0;
                    unitCard.evasion = 0;
                    unitCard.flying = false;

                    break;
            }

            cardGO.transform.SetParent(cardDrawPanel.transform.GetChild(0).transform.GetChild(x));
            cardGO.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            cardGO.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            cardGO.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            cardGO.AddComponent(typeof(CardUI));

            cardGO.GetComponent<Button>().onClick.AddListener(cardGO.GetComponent<CardUI>().addClickedCardToHand);
        }
    }
}