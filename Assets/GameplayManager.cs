using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class GameplayManager : Singleton<GameplayManager>
{
    public GameObject pathfindingGameObject;

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Material availablePosition;
    [SerializeField] private Material defaultMaterial;

    private const float lockAxis = 27f;

    public Player currentPlayer = default;

    private HashSet<Node> selectableNodes;

    public Pathfinding pf;
    public Grid grid;

    public GameObject tempKingUnit; // temporary king

    private Unit currentUnit;

    public bool cardSelected;

    public CardEffectManager cardEffectManager;

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

        // create initial temp king

        Unit kingUnit = tempKingUnit.GetComponent<Unit>();

        kingUnit.SetMovementSpeed(5);
        kingUnit.SetCanFly(false);
        kingUnit.SetUnitType(Unit.UnitTypes.King);
        kingUnit.SetUnitPlayerID(0);
        kingUnit.SetMaxHealth(30);
        kingUnit.SetCurrentHealth(30);
        kingUnit.SetDamage(7);
        kingUnit.SetDefence(2);
        kingUnit.SetMinRange(1);
        kingUnit.SetMaxRange(1);
        kingUnit.SetAccuracy(90);
        kingUnit.SetEvasion(30);

        // create initial temp king
        Node initialKingNode = grid.NodeFromWorldPoint(tempKingUnit.transform.position);
        initialKingNode.AddUnit(tempKingUnit.GetComponent<Unit>());

         cardSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTurnState == TurnState.Free)
        {
            if (cardSelected)
            {
                PlaceCard(currentPlayer, new Card(), 0);
            }
            else if (Input.GetMouseButtonDown(0))
                checkBoardClick();
        }
        if (currentTurnState == TurnState.SelectingTileMovement)
        {
            if (Input.GetMouseButtonDown(1))
            {
                validateSelectTileClickMove();
            }
        }
        if(currentTurnState == TurnState.SelectingCardOrigin)
        {
            if (Input.GetMouseButtonDown(1))
            {
                validateSelectTileClickCard();
            }
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

                    pf.depthLimit = unitMoveSpeed;
                    selectableNodes = pf.BFSLimitSearch(hit.transform.position, false, unitMoveSpeed);

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

        Card unitCard_soldier = new UnitCard(currentPlayer.PlayerId, CardType.Unit, "Unit: Soldier",/* GameObject cardImage,*/ 1, 1, 2, 10, 5, 1, 1, 1, 4, 80, 20, false);

        Card currentCard = unitCard_soldier;

        //temp set player id to 0
        currentPlayer.PlayerId = 0;

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

        currentTurnState = TurnState.SelectingCardOrigin;
    }

    void validateSelectTileClickCard()
    {
        print("i get here");
        Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
        Node selectedNode = grid.NodeFromWorldPoint(areaToInstantiate);
        if (selectableNodes.Contains(selectedNode))
        {
            print("valid selection");
            if (selectedNode.unitInThisNode == null)
            {
                cardEffectManager.createSoldierUnit(currentPlayer.PlayerId);
                ResetMaterial();
            }
            else
            {
                print("There is already a unit on this node!");
                ResetMaterial();
            }
        }
        else
        {
            print("invalid selection");
            ResetMaterial();
        }

        selectableNodes.Clear();
        currentTurnState = TurnState.Free;
        cardSelected = false;
    }
}
