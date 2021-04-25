using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using Photon.Pun;


public class TurnManager : MonoBehaviour, IPunObservable
{
    public static TurnManager instance = null;
    [SerializeField] private GameObject attackAnimationHit;
    [SerializeField] private GameObject healAnimation;
    [SerializeField] private GameObject attackAnimationMiss;
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
    public Node currentUnitNode;

    [SerializeField] private CardEffectManager cardEffectManager;

    private bool cardSelected;
    public Card storedCard;

    public bool cardSuccessful;

    public List<Unit> allUnits;

    public int currentMana;

    private GameObject animRef;

    public enum TurnState
    {
        DrawingCard,
        Waiting,
        Free,
        SelectingCardOrigin,
        SelectingTileMovement,
        SelectingTileAttack,
        SelectingTileHeal,
        PlacingEnemyUnit // temp for testing
    }

    public bool placingEnemyUnit; // temp for testing

    [SerializeField] public TurnState currentTurnState;

    [SerializeField] private GameObject gameplayPanel;
    private GameObject unitPanel;

    // Stat panel

    private TextMeshProUGUI unitPlayerIDText;
    private Image unitImage;
    private TextMeshProUGUI unitDamageText;
    private TextMeshProUGUI unitDefenceText;
    private TextMeshProUGUI unitARText;
    private TextMeshProUGUI unitMSText;
    private TextMeshProUGUI unitAccuracyText;
    private TextMeshProUGUI unitEvasionText;
    private TextMeshProUGUI unitHealthText;
    private Slider unitHealthBarSlider;

    // Option Menu
    private GameObject optionPanel;

    private GameObject attackButtonRef;
    private GameObject healButtonRef;

    // Card Draw

    [SerializeField] public GameObject cardPrefab;

    public GameObject cardDrawPanel;
    private GameObject handPanel;

    // Mana Panel
    private TextMeshProUGUI manaText;

    // Game History Panel
    private TextMeshProUGUI gameHistoryText;

    // Turn Update Panel
    private TextMeshProUGUI turnUpdateText;

    // Hovered Tile Panel
    [HideInInspector]
    public TextMeshProUGUI hoveredTileText;

    private const int COST_MOVE = 1;
    private const int COST_HEAL = 1;
    private const int COST_ATTACK = 2;

    // color32
    [HideInInspector]
    public Color32 color32_red = new Color32(255, 0, 0, 255);

    [HideInInspector]
    public Color32 color32_green = new Color32(0, 160, 0, 255);

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (GameManager.instance.networked)
        {
            PhotonView.Get(gameObject).ObservedComponents.Add(this);
        }

        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        grid = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();

        gameplayPanel = GameObject.Find("GameplayUICanvas");
        unitPanel = gameplayPanel.transform.GetChild(0).transform.GetChild(0).gameObject;

        unitPlayerIDText = unitPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        unitImage = unitPanel.transform.GetChild(0).GetComponent<Image>();

        unitDamageText = unitPanel.transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitDefenceText = unitPanel.transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitARText = unitPanel.transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitMSText = unitPanel.transform.GetChild(6).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitAccuracyText = unitPanel.transform.GetChild(7).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitEvasionText = unitPanel.transform.GetChild(8).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitHealthText = unitPanel.transform.GetChild(9).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unitHealthBarSlider = unitPanel.transform.GetChild(10).transform.GetChild(0).transform.GetChild(1).GetComponent<Slider>();

        optionPanel = gameplayPanel.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject;

        attackButtonRef = optionPanel.transform.GetChild(0).gameObject;
        healButtonRef = optionPanel.transform.GetChild(1).gameObject;

        manaText = gameplayPanel.transform.GetChild(0).transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        gameHistoryText = gameplayPanel.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        cardDrawPanel = gameplayPanel.transform.GetChild(0).transform.GetChild(6).gameObject;

        handPanel = gameplayPanel.transform.GetChild(0).transform.GetChild(3).gameObject;

        turnUpdateText = gameplayPanel.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        hoveredTileText = gameplayPanel.transform.GetChild(0).transform.GetChild(7).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        currentTurnState = TurnState.Free;

        selectableNodes = new HashSet<Node>();

        cardSelected = false;
        cardSuccessful = false;
        placingEnemyUnit = false;
        currentPlayer = GameLoop.instance.GetCurrentPlayer();
        localPlayer = GameLoop.instance.GetPlayerList()[0];

        if (PhotonNetwork.IsConnected)
        {
            foreach (NetworkedPlayer nPlayer in GameLoop.instance.GetPlayerList())
            {
                if (nPlayer.amIP1)
                {
                    if (PhotonNetwork.IsMasterClient)
                        localPlayer = nPlayer;
                }
                else
                {
                    if (!PhotonNetwork.IsMasterClient)
                        localPlayer = nPlayer;
                }
            }
        }
        
        loadPlayerHand();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameLoop.instance.GameOver)
            return;

        currentPlayer = GameLoop.instance.GetCurrentPlayer();

        if (GameManager.instance.networked)
        {
            Player thisPlayer = currentPlayer;
            manaText.text = "Mana " + thisPlayer.PlayerMana + "/" + thisPlayer.PlayerMaxMana;
        }
        else
        {
            Player thisPlayer = GameLoop.instance.GetPlayer(0);
            manaText.text = "Mana " + thisPlayer.PlayerMana + "/" + thisPlayer.PlayerMaxMana;
        }

        if (currentTurnState == TurnState.DrawingCard)
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
        else if (currentTurnState == TurnState.SelectingTileHeal)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickHeal();
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

        // destroy attack GameObject animation after it is finished playing
        // if (animRef != null)
        // {
        //     Destroy(animRef, animRef.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        // }
    }

    public void loadPlayerHand()
    {
        if (localPlayer.GetHand().Count <= 5)
        {
            handPanel.transform.GetChild(0).gameObject.SetActive(true);
            handPanel.transform.GetChild(1).gameObject.SetActive(false);

            foreach (Transform child in handPanel.transform.GetChild(0).gameObject.transform)
            {
                if (child.gameObject.transform.childCount > 0)
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
        else if (localPlayer.GetHand().Count > 5)
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

    public void summonEnemy()
    {
        currentTurnState = TurnState.PlacingEnemyUnit;
        placingEnemyUnit = true;
    }

    // temp function to place enemy units
    public void placeEnemyUnit()
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
            if (selectedNode.GetUnit() == null)
            {
                cardEffectManager.CreateUnit(Unit.UnitTypes.Soldier, selectedNode); // assuming enemy player id = 1
            }
        }
        currentTurnState = TurnState.Free;
        placingEnemyUnit = false;
    }

    public void unitMove()
    {
        if (currentUnit != null && currentUnit.GetUnitPlayerID() == currentPlayer.PlayerId)
        {
            if (!currentPlayer.ManaCheck(COST_MOVE))
            {
                updateTurnUpdate("Not enough mana to move a unit!", color32_red);
                return;
            }

            // Display All Valid Tiles to move on
            int unitMoveSpeed = currentUnit.GetMovementSpeedLeft();
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
            if (!currentUnit.GetCanAttack())
            {
                updateTurnUpdate("This unit cannot attack again for the rest of the turn.", color32_red);
                return;
            }
            else if (!currentPlayer.ManaCheck(COST_ATTACK))
            {
                updateTurnUpdate("Not enough mana to perform an attack!", color32_red);
                return;
            }

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

    public void unitHeal()
    {
        if (currentUnit != null && currentUnit.GetUnitPlayerID() == currentPlayer.PlayerId)
        {
            if (!currentUnit.GetCanAttack())
            {
                updateTurnUpdate("This unit cannot heal again for the rest of the turn.", color32_red);
                return;
            }
            else if (!currentPlayer.ManaCheck(COST_HEAL))
            {
                updateTurnUpdate("Not enough mana to perform a heal!", color32_red);
                return;
            }

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

                    newMat.material = availableMaterial;
                }
            }

            // set the units' layer to ignore raycast (used because the unit model stopped raycast from hitting tile, to show AOE)
            List<Node> allUnitsNodes = grid.GetAllUnitNodes();

            foreach (Node node in allUnitsNodes)
                node.GetUnit().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            currentTurnState = TurnState.SelectingTileHeal;
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
                bool clickCondition = PhotonNetwork.IsConnected
                    ? hit.transform.GetComponent<Unit>().GetUnitPlayerID() == currentPlayer.PlayerId
                    : hit.transform.GetComponent<Unit>().GetUnitPlayerID() == 0 && currentPlayer.PlayerId == 0;
                
                if (clickCondition) // check if unit belongs to current player
                {
                    currentUnit = hit.transform.GetComponent<Unit>();
                    currentUnitPosition = hit.transform.position;
                    currentUnitNode = grid.NodeFromWorldPoint(currentUnitPosition);

                    if (PhotonNetwork.IsConnected)
                    {
                        optionPanel.SetActive(PhotonNetwork.IsMasterClient == ((NetworkedPlayer)GameLoop.instance.GetCurrentPlayer()).amIP1);
                    }
                    else
                    {
                        optionPanel.SetActive(true);
                    }

                    if (currentUnit.GetUnitType() == Unit.UnitTypes.Priest)
                    {
                        attackButtonRef.SetActive(false);
                        healButtonRef.SetActive(true);
                    }
                    else
                    {
                        attackButtonRef.SetActive(true);
                        healButtonRef.SetActive(false);
                    }
                }
                else
                    optionPanel.SetActive(false);

                unitPlayerIDText.text = "" + hit.transform.GetComponent<Unit>().GetUnitType() + " (" + hit.transform.GetComponent<Unit>().GetUnitPlayerID() + ")";


                if (hit.transform.GetComponent<Unit>().GetUnitPlayerID() == 0) // player id 0
                {
                    if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Soldier)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Soldier_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Knight)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Knight_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Assassin)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Assassin_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Priest)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Priest_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Archer)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Archer_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.DragonRider)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/DragonRider_Blue");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.King)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/King_Blue");
                }
                else
                {
                    if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Soldier)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Soldier_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Knight)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Knight_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Assassin)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Assassin_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Priest)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Priest_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.Archer)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/Archer_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.DragonRider)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/DragonRider_Red");
                    else if (hit.transform.GetComponent<Unit>().GetUnitType() == Unit.UnitTypes.King)
                        unitImage.sprite = Resources.Load<Sprite>("CardImages/Unit/King_Red");
                }

                unitDamageText.text = "" + hit.transform.GetComponent<Unit>().GetDamage();
                unitDefenceText.text = "" + hit.transform.GetComponent<Unit>().GetDefence();
                unitARText.text = "" + hit.transform.GetComponent<Unit>().GetMinRange() + " - " + hit.transform.GetComponent<Unit>().GetMaxRange();
                unitMSText.text = "" + hit.transform.GetComponent<Unit>().GetMovementSpeed() + " (" + hit.transform.GetComponent<Unit>().GetMovementSpeedLeft() + ")";
                unitAccuracyText.text = "" + hit.transform.GetComponent<Unit>().GetAccuracy();
                unitEvasionText.text = "" + hit.transform.GetComponent<Unit>().GetEvasion();
                unitHealthText.text = "" + hit.transform.GetComponent<Unit>().GetCurrentHealth() + "/" + hit.transform.GetComponent<Unit>().GetMaxHealth();
                unitHealthBarSlider.value = (float)hit.transform.GetComponent<Unit>().GetCurrentHealth() / hit.transform.GetComponent<Unit>().GetMaxHealth();
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

        if (selectedNode != null && currentPlayer.ManaCheck(1))
        {
            if (selectableNodes.Contains(selectedNode) && selectedNode.GetUnit() == null)
            {
                currentUnit.isClicked = true;

                updateGameHistory("Player " + currentPlayer.PlayerId + " moved " + currentUnit.GetUnitType() + " (" + currentUnitNode.gridX + "," + currentUnitNode.gridY + ") to (" + selectedNode.gridX + "," + selectedNode.gridY + ")!\n");
                updateTurnUpdate("Successfully moved " + currentUnit.GetUnitType() + " (" + currentUnitNode.gridX + "," + currentUnitNode.gridY + ") to (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", color32_green);

                currentUnitPosition = selectedNodePosition;
                currentUnit.SelectNewUnitPosition();
                currentPlayer.PlayerMana -= COST_MOVE;

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

    void validateSelectTileClickHeal()
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
            if (selectedNode.GetUnit() != null && selectableNodes.Contains(selectedNode))
            {
                animRef = Instantiate(healAnimation, currentUnit.transform, false);
                selectedNode.GetUnit().IncreaseCurrentHealthBy(currentUnit.GetDamage());
                currentPlayer.PlayerMana -= COST_HEAL;
                currentUnit.SetCanAttack(false);
                updateGameHistory("Player " + currentPlayer.PlayerId + "'s Priest (" + currentUnitNode.gridX + "," + currentUnitNode.gridY + ") healed " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ") for " + currentUnitNode.GetUnit().GetDamage() + " health!\n");
                updateTurnUpdate("Successfully healed " + selectedNode.GetUnit().GetUnitType() + " (" + selectedNode.gridX + "," + selectedNode.gridY + ")!", color32_green);
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
        if (!currentPlayer.ManaCheck(1))
            return;

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

        if (selectedNode != null && selectableNodes.Contains(selectedNode))
        {
            if (selectedNode.GetUnit() != null)
            {
                if (GameManager.instance.networked)
                {
                    // move hit chance here to make sure its the same for both players clients
                    int hitChance = Mathf.Max(0, (int)Mathf.Floor(currentUnit.GetAccuracy() - selectedNode.GetUnit().GetEvasion() / 2));
                    int roll = Random.Range(0, 101); // generate 0-100

                    PhotonView.Get(gameObject).RequestOwnership();
                    PhotonView.Get(gameObject).RPC("NetworkAttack", RpcTarget.All, currentUnit.transform.position, selectedNode.worldPosition, roll <= hitChance);
                    //Attack(currentUnit, selectedNode.GetUnit());
                }
                else
                {
                    Attack(currentUnit, selectedNode.GetUnit());
                }
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


    void Attack(Unit attacker, Unit receiver)
    {
        int damageDealt = Mathf.Max(0, attacker.GetDamage() - receiver.GetDefence());

        int hitChance = Mathf.Max(0, (int)Mathf.Floor(attacker.GetAccuracy() - receiver.GetEvasion() / 2));
        int roll = Random.Range(0, 101); // generate 0-100

        Node attackerNode = grid.NodeFromWorldPoint(attacker.transform.position);
        Node receiverNode = grid.NodeFromWorldPoint(receiver.transform.position);

        if (roll <= hitChance)
        {
            receiver.SetCurrentHealth(receiver.GetCurrentHealth() - damageDealt);
            updateGameHistory("Player " + currentPlayer.PlayerId + "'s " + currentUnitNode.GetUnit().GetUnitType() + "(" + attackerNode.gridX + "," + attackerNode.gridY + ") attacked " + receiver.GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ") for " + attackerNode.GetUnit().GetDamage() + " damage!\n");
            updateTurnUpdate("Successfully attacked " + receiverNode.GetUnit().GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ")!", color32_green);
            animRef = Instantiate(attackAnimationHit, currentUnit.transform, false);
        }
        else
        {
            updateGameHistory("Player " + currentPlayer.PlayerId + "'s " + currentUnitNode.GetUnit().GetUnitType() + "(" + attackerNode.gridX + "," + attackerNode.gridY + ") missed an attack on " + receiver.GetUnitType() + " (" + receiverNode.gridX + "," + attackerNode.gridY + ")!\n");
            updateTurnUpdate("Missed an attacked on " + receiverNode.GetUnit().GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ")!");
            animRef = Instantiate(attackAnimationMiss, currentUnit.transform, false);
        }

        attacker.SetCanAttack(false);
        currentPlayer.PlayerMana -= COST_ATTACK;
    }

    [PunRPC]
    void NetworkAttack(Vector3 attackerPos, Vector3 receiverPos, bool hit)
    {
        print("We attacking");
        Unit attacker = grid.NodeFromWorldPoint(attackerPos).GetUnit();
        Unit receiver = grid.NodeFromWorldPoint(receiverPos).GetUnit();

        int damageDealt = Mathf.Max(0, attacker.GetDamage() - receiver.GetDefence());

        // int hitChance = Mathf.Max(0, (int)Mathf.Floor(attacker.GetAccuracy() - receiver.GetEvasion() / 2));
        // int roll = Random.Range(0, 101); // generate 0-100

        Node attackerNode = grid.NodeFromWorldPoint(attacker.transform.position);
        Node receiverNode = grid.NodeFromWorldPoint(receiver.transform.position);

        if (hit)
        {
            receiver.SetCurrentHealth(receiver.GetCurrentHealth() - damageDealt);
            if (PhotonView.Get(gameObject).IsMine)
            {
                updateGameHistory("Player " + currentPlayer.PlayerId + "'s " + currentUnitNode.GetUnit().GetUnitType() + "(" + attackerNode.gridX + "," + attackerNode.gridY + ") attacked " + receiver.GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ") for " + attackerNode.GetUnit().GetDamage() + " damage!\n");
                updateTurnUpdate("Successfully attacked " + receiverNode.GetUnit().GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ")!", color32_green);
                animRef = PhotonNetwork.Instantiate("UnitAnimation/Blast_Hit", currentUnit.transform.position, Quaternion.identity);
            }
        }
        else
        {
            if (PhotonView.Get(gameObject).IsMine)
            {
                updateGameHistory("Player " + currentPlayer.PlayerId + "'s " + currentUnitNode.GetUnit().GetUnitType() + "(" + attackerNode.gridX + "," + attackerNode.gridY + ") missed an attack on " + receiver.GetUnitType() + " (" + receiverNode.gridX + "," + attackerNode.gridY + ")!\n");
                updateTurnUpdate("Missed an attacked on " + receiverNode.GetUnit().GetUnitType() + " (" + receiverNode.gridX + "," + receiverNode.gridY + ")!");
                animRef = PhotonNetwork.Instantiate("UnitAnimation/Blast_Miss", currentUnit.transform.position, Quaternion.identity);
            }
        }

        attacker.SetCanAttack(false);
        currentPlayer.PlayerMana -= COST_ATTACK;
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
        GameObject clickedButtonGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject; // gets object that calls onClick
        Card currentCard = clickedButtonGO.GetComponent<Card>();
        storedCard = currentCard;

        if (!currentPlayer.ManaCheck(storedCard.cost))
        {
            updateTurnUpdate("Not enough mana to cast this card!", color32_red);
            return;
        }

        if (storedCard.id == 20) // greed card
        {
            cardEffectManager.spell_greed(currentPlayer.PlayerId);
            if (cardSuccessful)
                currentPlayer.RemoveCard(storedCard);

            cardSuccessful = false;

            loadPlayerHand();
            currentTurnState = TurnState.Free;
            cardSelected = false;
            currentPlayer.PlayCard(storedCard);
            return;
        }

        if (grid != null)
        {
            if (storedCard.id == 7) // snipe     
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard, Unit.UnitTypes.Archer));
            else if (storedCard.id == 9) // prayer
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard, Unit.UnitTypes.Priest));
            else if (storedCard.id == 21) // Warcry
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard, Unit.UnitTypes.Knight));
            else if (storedCard.id == 23) // Assassinate
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard, Unit.UnitTypes.Assassin));
            else if (storedCard.id == 26) // Royal Pledge
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard, Unit.UnitTypes.King));
            else
                selectableNodes.UnionWith(Grid.instance.GetPlaceableNodes(currentCard));

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
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                selectedNode = grid.NodeFromWorldPoint(hit.transform.position);
                selectedNodePosition = hit.transform.position;
            }
        }
        else
        {
            return;
        }

        if (selectableNodes.Contains(selectedNode))
        {
            if (!currentPlayer.ManaCheck(storedCard.cost))
                return;

            if (storedCard.castType == CastType.OnUnit)
            {
                if (selectedNode.GetUnit() != null)
                {
                    if (storedCard.id == 10)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_vitality", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_vitality(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 11)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_endurance", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_endurance(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 12)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_vigor", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_vigor(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 13)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_nimbleness", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_nimbleness(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 14)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_agility", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_agility(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 15)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_precision", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_precision(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 18)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_provisions", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_provisions(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 19)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_reinforcements", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_reinforcements(currentPlayer.PlayerId, selectedNodePosition);
                        }
                    }
                    else if (storedCard.id == 21)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_warcry", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_warcry(currentPlayer.PlayerId, selectedNodePosition);
                        }
                    }
                    else if (storedCard.id == 22)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_rebirth", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_rebirth(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 26)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_royalPledge", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_royalPledge(currentPlayer.PlayerId, selectedNodePosition);
                        }
                    }
                    else if (storedCard.id == 6)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_smite", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_smite(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 7)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_snipe", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_snipe(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                    else if (storedCard.id == 23)
                    {
                        if (GameManager.instance.networked)
                        {
                            PhotonView.Get(cardEffectManager).RPC("spell_assassinate", RpcTarget.All, currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                        else
                        {
                            cardEffectManager.spell_assassinate(currentPlayer.PlayerId, selectedNode.worldPosition);
                        }
                    }
                }
            }
            else if (storedCard.castType == CastType.OnEmpty)
            {
                if (storedCard as UnitCard)
                {
                    if (selectedNode.GetUnit() == null)
                        cardEffectManager.CreateUnit(((UnitCard)storedCard).UnitType, selectedNode);
                    else
                        updateTurnUpdate("This cell is not empty!", TurnManager.instance.color32_red);
                }
                else
                {
                    if (selectedNode.GetUnit() == null)
                    {
                        if (storedCard.id == 16)
                        {
                            if (GameManager.instance.networked)
                            {
                                PhotonView.Get(cardEffectManager).RPC("spell_oracle", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                            }
                            else
                            {
                                cardEffectManager.spell_oracle(currentPlayer.PlayerId, selectedNodePosition);
                            }
                        }
                        else if (storedCard.id == 17)
                        {
                            if (GameManager.instance.networked)
                            {
                                PhotonView.Get(cardEffectManager).RPC("spell_disarmTrap", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                            }
                            else
                            {
                                cardEffectManager.spell_disarmTrap(currentPlayer.PlayerId, selectedNodePosition);
                            }
                        }
                        else if (storedCard.id == 24)
                        {
                            if (GameManager.instance.networked)
                            {
                                PhotonView.Get(cardEffectManager).RPC("spell_bearTrap", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                            }
                            else
                            {
                                cardEffectManager.spell_bearTrap(currentPlayer.PlayerId, selectedNode.worldPosition);
                            }
                        }
                        else if (storedCard.id == 25)
                        {
                            if (GameManager.instance.networked)
                            {
                                PhotonView.Get(cardEffectManager).RPC("spell_landMine", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                            }
                            else
                            {
                                cardEffectManager.spell_landMine(currentPlayer.PlayerId, selectedNode.worldPosition);
                            }
                        }
                    }
                }
            }
            else if (storedCard.castType == CastType.OnAny)
            {
                if (storedCard.id == 8)
                    if (GameManager.instance.networked)
                    {
                        PhotonView.Get(cardEffectManager).RPC("spell_heavenlySmite", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                    }
                    else
                    {
                        cardEffectManager.spell_heavenlySmite(currentPlayer.PlayerId, selectedNodePosition);
                    }
                else if (storedCard.id == 9)
                {
                    if (GameManager.instance.networked)
                    {
                        PhotonView.Get(cardEffectManager).RPC("spell_prayer", RpcTarget.All, currentPlayer.PlayerId, selectedNodePosition);
                    }
                    else
                    {
                        cardEffectManager.spell_prayer(currentPlayer.PlayerId, selectedNodePosition);
                    }
                }
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

        if (cardSuccessful)
            currentPlayer.PlayCard(storedCard);

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

        unitImage.sprite = null;

        unitDamageText.text = "-";
        unitDefenceText.text = "-";
        unitARText.text = "x - x";
        unitMSText.text = "-";
        unitAccuracyText.text = "-";
        unitEvasionText.text = "-";
        unitHealthText.text = "-/-";
        unitHealthBarSlider.value = 0f;

        optionPanel.SetActive(false);
    }

    public void ShowCardSelection()
    {
        cardDrawPanel.SetActive(true);

        UnitCard unitCard;
        SpellCard spellCard;
        GameObject cardGO;

        for (int x = 1; x < 6; x++)
        {
            if (cardDrawPanel.transform.GetChild(0).transform.GetChild(x).childCount > 0)
                Destroy(cardDrawPanel.transform.GetChild(0).transform.GetChild(x).transform.GetChild(0).gameObject);

            RectTransform rt = cardDrawPanel.transform.GetChild(0).transform.GetChild(x).GetComponent<RectTransform>();

            /*Top*/
            rt.offsetMax = new Vector2(0, 0);
            /*Bottom*/
            rt.offsetMin = new Vector2(0, 0);

            cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

            Card randomCard = RandomCard();

            if (randomCard.GetType() == typeof(UnitCard))
            {
                UnitCard randomCardComponent = (UnitCard)cardGO.AddComponent(randomCard.GetType());
                randomCardComponent.copyUnitCard((UnitCard)randomCard);
            }
            else if (randomCard.GetType() == typeof(SpellCard))
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

    //Would be much easier if this was modular and hardcoding all the unit stats wasnt a requirement. I would suggets 
    //copying unit data from a Unit prefab or a spell prefab. Otherwise if you change any unit stats you have to change
    //it in multiple places
    public Card RandomCard()
    {
        int random = Random.Range(0, 27);

        if (random < 6)
        {
            UnitCard card = new UnitCard();
            switch (random)
            {
                case 0:
                    card.UnitType = Unit.UnitTypes.Soldier;
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
                    card.UnitType = Unit.UnitTypes.Knight;
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
                    card.UnitType = Unit.UnitTypes.Assassin;
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
                    card.UnitType = Unit.UnitTypes.Priest;
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
                    card.UnitType = Unit.UnitTypes.Archer;
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
                    card.UnitType = Unit.UnitTypes.DragonRider;
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
                    card.flying = true;

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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnAny;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.name = "Greed";
                    card.cost = 3;
                    card.minRange = 0;
                    card.maxRange = 0;
                    card.aoeMinRange = 0;
                    card.aoeMaxRange = 0;
                    card.description = "Draw 2 cards.";

                    break;

                case 21:



                    card.id = 21;
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnUnit;
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
                    card.castType = CastType.OnEmpty;
                    card.name = "Bear Trap";
                    card.cost = 2;
                    card.minRange = 1;
                    card.maxRange = 2;
                    card.aoeMinRange = 0;
                    card.aoeMaxRange = 0;
                    card.description = "Damages the triggering unit for 5 health.";

                    break;

                case 25:



                    card.id = 25;
                    card.castType = CastType.OnEmpty;
                    card.name = "Land Mine";
                    card.cost = 3;
                    card.minRange = 1;
                    card.maxRange = 2;
                    card.aoeMinRange = 0;
                    card.aoeMaxRange = 1;
                    card.description = "Damages all units from (0,1) tiles from the detonation origin for 3 health.";

                    break;

                case 26:



                    card.id = 26;
                    card.castType = CastType.OnUnit;
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

    // method to update game history (can maybe make this a PunRPC to have the same on both clients
    public void updateGameHistory(string actionString)
    {
        if (GameManager.instance.networked)
            PhotonView.Get(gameObject).RequestOwnership();

        gameHistoryText.text += System.DateTime.Now.ToString("[HH:mm:ss] ") + actionString;
    }

    public void updateTurnUpdate(string message)
    {
        turnUpdateText.text = message;
        turnUpdateText.color = new Color32(0, 0, 0, 255);

    }
    public void updateTurnUpdate(string message, Color32 color)
    {
        turnUpdateText.text = message;
        turnUpdateText.color = color;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameHistoryText.text);
        }
        else
        {
            gameHistoryText.text = (string)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void signalPlayers(Vector3 triggeringOriginNodePosition, TrapOrItem.TrapOrItemTypes trapType)
    {
        Node triggeringOriginNode = grid.NodeFromWorldPoint(triggeringOriginNodePosition);

        if (trapType == TrapOrItem.TrapOrItemTypes.BearTrap)
        {
            // TIMING ISSUE
            //triggeringOriginNode.GetUnit().SetCurrentHealth(triggeringOriginNode.GetUnit().GetCurrentHealth() - 5);
            TurnManager.instance.updateGameHistory("Bear trap triggered at (" + triggeringOriginNode.gridX + "," + triggeringOriginNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Bear trap triggered!");
        }
        else if (trapType == TrapOrItem.TrapOrItemTypes.LandMine)
        {
            /*HashSet<Node> affectedNodes = pf.GetNodesMinMaxRange(triggeringOriginNode.worldPosition, false, 0, 1);

            foreach (Node node in affectedNodes)
            {
                if (node.GetUnit() != null)
                    node.GetUnit().SetCurrentHealth(triggeringOriginNode.GetUnit().GetCurrentHealth() - 3);
            }*/
            TurnManager.instance.updateGameHistory("Land Mine triggered at (" + triggeringOriginNode.gridX + "," + triggeringOriginNode.gridY + ")!\n");
            TurnManager.instance.updateTurnUpdate("Land Mine triggered!");
        }
    }
}