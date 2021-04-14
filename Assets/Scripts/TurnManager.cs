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

			cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
			
			Card randomCard = RandomCard();
			
			if(randomCard.GetType() == typeof(UnitCard))
			{
				UnitCard randomCardComponent = (UnitCard)cardGO.AddComponent(randomCard.GetType());
				randomCardComponent.copyUnitCard((UnitCard)randomCard);
			}
			else if(randomCard.GetType() == typeof(SpellCard))
			{
				SpellCard randomCardComponent = (SpellCard)cardGO.AddComponent(randomCard.GetType());
				randomCardComponent.copySpellCard((SpellCard)randomCard);
			}
			else
			{
				Debug.LogError("Card type error! : " + randomCard.GetType().ToString());
			}
			
            cardGO.transform.SetParent(cardDrawPanel.transform.GetChild(0).transform.GetChild(x));
            cardGO.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            cardGO.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            cardGO.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            cardGO.AddComponent(typeof(CardUI));

            cardGO.GetComponent<Button>().onClick.AddListener(cardGO.GetComponent<CardUI>().addClickedCardToHand);
        }
    }
	
	private Card RandomCard()
	{
		int random = Random.Range(0, 28);

		if(random < 6)
		{
			UnitCard card = new UnitCard();
			switch (random)
			{
				case 0:

					card.id = 0;
					card.castType = CastType.OnEmpty;
					card.name = "Soldier";
					card.cost = 1;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 10;
					card.damage = 5;
					card.defence = 1;
					card.minAttackRange = 1;
					card.maxAttackRange = 1;
					card.moveSpeed = 4;
					card.accuracy = 80;
					card.evasion = 20;
					card.flying = false;

					break;

				case 1:

					card.id = 1;
					card.castType = CastType.OnEmpty;
					card.name = "Knight";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 20;
					card.damage = 7;
					card.defence = 3;
					card.minAttackRange = 1;
					card.maxAttackRange = 1;
					card.moveSpeed = 3;
					card.accuracy = 70;
					card.evasion = 10;
					card.flying = false;

					break;

				case 2:

					card.id = 2;
					card.castType = CastType.OnEmpty;
					card.name = "Assassin";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 15;
					card.damage = 9;
					card.defence = 0;
					card.minAttackRange = 1;
					card.maxAttackRange = 1;
					card.moveSpeed = 6;
					card.accuracy = 95;
					card.evasion = 60;
					card.flying = false;

					break;

				case 3:

					card.id = 3;
					card.castType = CastType.OnEmpty;
					card.name = "Priest";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 15;
					card.damage = 5;
					card.defence = 0;
					card.minAttackRange = 0;
					card.maxAttackRange = 2;
					card.moveSpeed = 4;
					card.accuracy = 100;
					card.evasion = 30;
					card.flying = false;

					break;

				case 4:

					card.id = 4;
					card.castType = CastType.OnEmpty;
					card.name = "Archer";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 15;
					card.damage = 6;
					card.defence = 0;
					card.minAttackRange = 2;
					card.maxAttackRange = 3;
					card.moveSpeed = 4;
					card.accuracy = 90;
					card.evasion = 30;
					card.flying = false;

					break;

				case 5:

					card.id = 5;
					card.castType = CastType.OnEmpty;
					card.name = "Dragon Rider";
					card.cost = 5;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.health = 25;
					card.damage = 6;
					card.defence = 2;
					card.minAttackRange = 1;
					card.maxAttackRange = 1;
					card.moveSpeed = 4;
					card.accuracy = 85;
					card.evasion = 20;
					card.flying = false;

					break;
			}
			
			return card;
		}
		else 
		{
			SpellCard card = new SpellCard();
			switch (random)
			{
				case 6:



					card.id = 6;
					card.castType = CastType.OnEnemy;
					card.name = "Smite";
					card.cost = 2;
					card.minRange = 1;
					card.maxRange = 1;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Damages an enemy unit for 5 health.";

					break;

				case 7:



					card.id = 7;
					card.castType = CastType.OnEnemy;
					card.name = "Snipe";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 3;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Only castable from a friendly Archer unit. Damages an enemy unit for 10 health.";

					break;

				case 8:



					card.id = 8;
					card.castType = CastType.OnAny;
					card.name = "Heavenly Smite";
					card.cost = 5;
					card.minRange = 0;
					card.maxRange = 3;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 1;
					card.description = "Damages all enemies within (0,1) tiles of the casting origin for 3 health.";

					break;

				case 9:



					card.id = 9;
					card.castType = CastType.OnAny;
					card.name = "Prayer";
					card.cost = 4;
					card.minRange = 0;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 1;
					card.description = "Only castable from a friendly Priest unit. Heal all allies within (0,1) tiles of the casting origin for 3 health.";

					break;

				case 10:



					card.id = 10;
					card.castType = CastType.OnAlly;
					card.name = "Vitality";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s current and maximum Health by 5.";

					break;

				case 11:



					card.id = 11;
					card.castType = CastType.OnAlly;
					card.name = "Endurance";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s Defence by 2.";

					break;

				case 12:



					card.id = 12;
					card.castType = CastType.OnAlly;
					card.name = "Vigor";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s Damage by 3.";

					break;

				case 13:



					card.id = 13;
					card.castType = CastType.OnAlly;
					card.name = "Nimbleness";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s movement speed by 1.";

					break;

				case 14:



					card.id = 14;
					card.castType = CastType.OnAlly;
					card.name = "Agility";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s Evasion by 10.";

					break;

				case 15:



					card.id = 15;
					card.castType = CastType.OnAlly;
					card.name = "Precision";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Increases an ally unit’s Accuracy by 10.";

					break;

				case 16:



					card.id = 16;
					card.castType = CastType.OnEmpty;
					card.name = "Oracle";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 5;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 2;
					card.description = "Reveals all traps within (0,2) tiles of the triggering origin.";

					break;

				case 17:



					card.id = 17;
					card.castType = CastType.OnEmpty;
					card.name = "Disarm Trap";
					card.cost = 1;
					card.minRange = 1;
					card.maxRange = 3;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Attempts to disarm an enemy trap on a tile.";

					break;

				case 18:



					card.id = 18;
					card.castType = CastType.OnAlly;
					card.name = "Provisions";
					card.cost = 1;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Heals a unit for 5 health.";

					break;

				case 19:



					card.id = 19;
					card.castType = CastType.OnAlly;
					card.name = "Reinforcements";
					card.cost = 6;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Summons 4 Soldier units around an ally.";

					break;

				case 20:



					card.id = 20;
					card.castType = CastType.OnAny;
					card.name = "Reinforcements";
					card.cost = 3;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Draw 2 cards.";

					break;

				case 21:



					card.id = 21;
					card.castType = CastType.OnAlly;
					card.name = "Warcry";
					card.cost = 4;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 1;
					card.description = "Only castable from a friendly Knight unit. Increases the damage of all allies within (0,1) tiles of the casting origin by 2.";

					break;

				case 22:



					card.id = 22;
					card.castType = CastType.OnAlly;
					card.name = "Rebirth";
					card.cost = 4;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Fully heals an ally unit.";

					break;

				case 23:



					card.id = 23;
					card.castType = CastType.OnEnemy;
					card.name = "Assassinate";
					card.cost = 5;
					card.minRange = 1;
					card.maxRange = 1;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Only castable from a friendly Assassin unit. Immediately kills and enemy unit (does not work on King unit).";

					break;

				case 24:



					card.id = 24;
					card.castType = CastType.OnAlly;
					card.name = "Teleport";
					card.cost = 1;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Teleports a unit (1,2) tiles.";

					break;

				case 25:



					card.id = 25;
					card.castType = CastType.OnEmpty;
					card.name = "Bear Trap";
					card.cost = 2;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Damages the triggering unit for 5 health.";

					break;

				case 26:



					card.id = 26;
					card.castType = CastType.OnEmpty;
					card.name = "Land Mine";
					card.cost = 3;
					card.minRange = 1;
					card.maxRange = 2;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 1;
					card.description = "Damages all units from (0,1) tiles from the detonation origin for 3 health.";

					break;

				case 27:



					card.id = 27;
					card.castType = CastType.OnAlly;
					card.name = "Royal Pledge";
					card.cost = 3;
					card.minRange = 0;
					card.maxRange = 0;
					card.aoeMinRange = 0;
					card.aoeMaxRange = 0;
					card.description = "Only castable from a friendly King unit. Increases the damage, current health and max health of all allies within (1,1) tiles of the casting origin by 2.";

					break;


			}
			
			return card;
		}
		
		return new Card();
	}
}