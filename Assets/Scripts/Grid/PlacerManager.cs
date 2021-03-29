using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

/*
 *  Verify card placments and perform card actions
 */

public class PlacerManager : Singleton<PlacerManager>
{
    [SerializeField] private Unit unitCreation;
    [SerializeField] private Material availablePosition;
    [SerializeField] private Material defaultMaterial;

    private bool placerClicked = false;
    private const float lockAxis = 27f;
    private Player playerPlacing = default;

    private Pathfinding pathfinding;
    private Grid grid;

    public CardEffectManager cardEffectManager;

    public GameObject tempKingUnit;
    HashSet<Node> selectableNodes;
    public bool currentlySelectingTile;

    void Awake()
    {
        pathfinding = GameObject.Find("Pathfinding").GetComponent<Pathfinding>();
        grid = pathfinding.gridRef;

        selectableNodes = new HashSet<Node>();

        currentlySelectingTile = false;

        // create initial temp king
        Node initialKingNode = grid.NodeFromWorldPoint(tempKingUnit.transform.position);
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
        initialKingNode.AddUnit(tempKingUnit.GetComponent<Unit>());
    }

    public void CreateUnit(Player player)
    {
        playerPlacing = player;
        placerClicked = true;
    }

    public void PlaceCard(Player currentPlayer, Card card, int cardIndex, Vector3 targetLocation)
    {

        // create temp card

        Card unitCard_soldier = new UnitCard(0, CardType.Unit, "Unit: Soldier",/* GameObject cardImage,*/ 1, 1, 2, 10, 5, 1, 1, 1, 4, 80, 20, false);

        Card currentCard = unitCard_soldier;

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
                    selectableNodes.UnionWith(pathfinding.GetNodesMinMaxRange(nodePos, false, 1, 2));
                }

                if (selectableNodes != null && selectableNodes.Count > 0)
                {
                    currentlySelectingTile = true;

                    foreach (var node in selectableNodes)
                    {
                        Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                        newMat.material = availablePosition;
                    }
                }
            }
        }


        //TODO: replace with check for type of card instead of just using a unit (e.i. spells or traps too)
        /*Unit unitPlaced = Instantiate(unitCreation, targetLocation, Quaternion.identity);
        currentPlayer.RemoveCard(cardIndex);
        currentPlayer.AddUnit(unitPlaced);

        playerPlacing = currentPlayer;
        placerClicked = true;*/
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


    private void Update()
    {

        if (placerClicked)
        {
            Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
            
            //TODO: will have to use real card and real card index
            PlaceCard(playerPlacing, new Card(), 0, areaToInstantiate); 
            placerClicked = false;
        }
        
        if(currentlySelectingTile && Input.GetMouseButtonDown(0))
        {
            print("i get here");
            Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
            Node selectedNode = grid.NodeFromWorldPoint(areaToInstantiate);
            if(selectableNodes.Contains(selectedNode))
            {
                print("valid selection");
                if (selectedNode.unitInThisNode == null)
                {
                    cardEffectManager.createSoldierUnit(playerPlacing.PlayerId);
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

            currentlySelectingTile = false;
        }
    }
    // for testing, allows to drag a unit with mouse, but might not be needed as can just click on square and instantiate.
    // private Vector3 mOffSet;
    // private float mZCoord;
    // private void OnMouseDown()
    // {
    //     mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
    //     
    //     // stores offset = gameobject world pos - mouse world pos
    //     mOffSet = gameObject.transform.position - GetMouseWorldPos();
    // }
    //
    // private Vector3 GetMouseWorldPos()
    // {
    //     // pixel coordinates (x, y)
    //     Vector3 mousePoint = Input.mousePosition;
    //     
    //     // z coordinate of game object on screen
    //     mousePoint.z = mZCoord;
    //
    //     return Camera.main.ScreenToWorldPoint(mousePoint);
    // }
    //
    // private void OnMouseDrag()
    // {
    //     transform.position = GetMouseWorldPos() + mOffSet;
    // }

}