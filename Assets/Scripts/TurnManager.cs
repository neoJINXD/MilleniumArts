using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class TurnManager : Singleton<TurnManager>
{
    public GameObject pathfindingGameObject;

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Material availablePosition;
    [SerializeField] private Material defaultMaterial;

    private const float lockAxis = 27f;

    public Player currentPlayer = default;

    private HashSet<Node> selectableNodes;

    private Pathfinding pf;
    private Grid grid;

    private Unit currentUnit;

    public bool cardSelected;

    public CardEffectManager cardEffectManager;

    private Card storedCard;

    enum TurnState
    {
        Waiting,
        Free,
        SelectingCardOrigin,
        SelectingTileMovement,
        SelectingTileAttack
    }

    [SerializeField] private TurnState currentTurnState;

    void Start()
    {
        pf = pathfindingGameObject.GetComponent<Pathfinding>();
        grid = pf.gridRef;

        currentTurnState = TurnState.Free;

        selectableNodes = new HashSet<Node>();

        cardSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTurnState == TurnState.Free)
        {
            if (cardSelected)
                PlaceCard(currentPlayer, new Card(), 0);
            else if (Input.GetMouseButtonDown(0))
                checkBoardClick();
        }
        else if (currentTurnState == TurnState.SelectingTileMovement)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickMove();
        }
        else if (currentTurnState == TurnState.SelectingCardOrigin)
        {
            if (Input.GetMouseButtonDown(0))
                validateSelectTileClickCard();
        }
    }

    void checkBoardClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // shoot raycast towards board
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Unit")) // if board hits unit
            {
                if (hit.transform.GetComponent<Unit>().GetUnitPlayerID() == 0) // check if unit belongs to current player
                {
                    currentUnit = hit.transform.GetComponent<Unit>();

                    // Display All Valid Tiles to move on
                    int unitMoveSpeed = (int)currentUnit.GetMovementSpeed();
                    Node currentNode = grid.NodeFromWorldPoint(currentUnit.transform.position);

                    selectableNodes = pf.GetNodesMinMaxRange(hit.transform.position, false, 0, unitMoveSpeed);

                    if (selectableNodes != null && selectableNodes.Count > 0)
                    {
                        foreach (var node in selectableNodes)
                        {
                            Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                            newMat.material = availablePosition;
                        }
                    }

                    currentTurnState = TurnState.SelectingTileMovement;
                }
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
            print("selected node x,y: " + selectedNode.gridX + ", " + selectedNode.gridY);
            if (selectedNode.unitInThisNode == null)
            {
                currentUnit.isClicked = true;
                currentUnit.SelectNewUnitPosition();
            }
        }
        ResetMaterial();
        selectableNodes.Clear();
        currentTurnState = TurnState.Free;
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

    public void PlaceCard(Player currentPlayer, Card card, int cardIndex)
    {
        print("running");
        // create temp card
        Card unitCard_soldier = new UnitCard(0, CardType.Unit, "Unit: Soldier",/* GameObject cardImage,*/ 1, 1, 2, 10, 5, 1, 1, 1, 4, 80, 20, false);
        Card unitCard_knight = new UnitCard(1, CardType.Unit, "Unit: Knight",/* GameObject cardImage,*/ 3, 1, 2, 30, 7, 4, 1, 1, 3, 70, 10, false);
        Card unitCard_assassin = new UnitCard(2, CardType.Unit, "Unit: Assassin",/* GameObject cardImage,*/ 3, 1, 2, 15, 9, 0, 1, 1, 8, 95, 60, false);
        Card unitCard_priest = new UnitCard(3, CardType.Unit, "Unit: Priest",/* GameObject cardImage,*/ 1, 1, 2, 15, 5, 0, 0, 2, 4, 100, 30, false);
        Card unitCard_archer = new UnitCard(4, CardType.Unit, "Unit: Archer",/* GameObject cardImage,*/ 1, 1, 2, 15, 6, 0, 2, 3, 4, 90, 30, false);
        Card unitCard_dragonRider = new UnitCard(5, CardType.Unit, "Unit: DragonRider",/* GameObject cardImage,*/ 1, 1, 2, 25, 6, 2, 1, 1, 6, 85, 20, true);

        Card activeCard_smite = new SpellCard(6, CardType.Active, "Smite", /*GameObject cardImage,*/ 1, 1, 1, "Damages an enemy unit for 5 health.");



        Card currentCard = activeCard_smite;
        storedCard = currentCard;


        //temp set player id to 0
        currentPlayer.PlayerId = 0;
        // for units
        if (currentCard.type == CardType.Unit)
        {
            if (grid != null)
            {
                List<Node> allyNodes = grid.GetAllyUnitNodes(currentPlayer.PlayerId); // hard code player 0
                print(allyNodes.Count);
                foreach (Node node in allyNodes)
                {
                    Vector3 nodePos = node.unitInThisNode.transform.position;
                    selectableNodes.UnionWith(pf.GetNodesMinMaxRange(nodePos, false, 1, 2));
                }

                if (selectableNodes != null && selectableNodes.Count > 0)
                {
                    foreach (var node in selectableNodes)
                    {
                        Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                        newMat.material = availablePosition;
                    }
                }
            }
        }
        else if (currentCard.type == CardType.Active)
        {
            if (grid != null)
            {
                List<Node> allyNodes = grid.GetAllyUnitNodes(currentPlayer.PlayerId); // hard code player 0
                print(allyNodes.Count);
                foreach (Node node in allyNodes)
                {
                    Vector3 nodePos = node.unitInThisNode.transform.position;
                    selectableNodes.UnionWith(pf.GetNodesMinMaxRange(nodePos, false, currentCard.minRange, currentCard.maxRange));
                }

                if (selectableNodes != null && selectableNodes.Count > 0)
                {
                    foreach (var node in selectableNodes)
                    {
                        Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                        newMat.material = availablePosition;
                    }
                }
            }
        }

        currentTurnState = TurnState.SelectingCardOrigin;
    }

    void validateSelectTileClickCard()
    {
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
        Node selectedNode = grid.NodeFromWorldPoint(areaToInstantiate);

        if (storedCard.type == CardType.Unit)
        {
            if (selectableNodes.Contains(selectedNode))
            {
                if (selectedNode.unitInThisNode == null)
                {
                    if (storedCard.id == 0) // spawn soldier
                        cardEffectManager.createSoldierUnit(currentPlayer.PlayerId);
                    else if (storedCard.id == 1) // spawn Knight
                        cardEffectManager.createKnightUnit(currentPlayer.PlayerId);
                    else if (storedCard.id == 2) // spawn Assassin
                        cardEffectManager.createAssassinUnit(currentPlayer.PlayerId);
                    else if (storedCard.id == 3) // spawn Priest
                        cardEffectManager.createPriestUnit(currentPlayer.PlayerId);
                    else if (storedCard.id == 4) // spawn Archer
                        cardEffectManager.createArcherUnit(currentPlayer.PlayerId);
                    else if (storedCard.id == 5) // spawn Dragon Rider
                        cardEffectManager.createDragonRiderUnit(currentPlayer.PlayerId);
                }
            }

        }
        else if (storedCard.type == CardType.Active)
        {
            if (selectableNodes.Contains(selectedNode))
            {
                if (selectedNode.unitInThisNode != null)
                {
                    print("i get here");
                    cardEffectManager.spell_smite(currentPlayer.PlayerId);
                }
            }
        }
        ResetMaterial();
        selectableNodes.Clear();
        currentTurnState = TurnState.Free;
        cardSelected = false;
    }
}