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
    private bool placerClicked = false;
    private const float lockAxis = 27f;
    private Player playerPlacing = default;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && placerClicked)
        {
            Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
            
            //TODO: will have to use real card and real card index
            PlaceCard(playerPlacing, new Card(), 0, areaToInstantiate); 
            placerClicked = false;
        }
    }
    public void CreateUnit(Player player)
    {
        playerPlacing = player;
        placerClicked = true;
    }

    public void PlaceCard(Player currentPlayer, Card card, int cardIndex, Vector3 targetLocation)
    {
        //TODO: replace with check for type of card instead of just using a unit (e.i. spells or traps too)
        Unit unitPlaced = Instantiate(unitCreation, targetLocation, Quaternion.identity);
        currentPlayer.RemoveCard(cardIndex);
        currentPlayer.AddUnit(unitPlaced);
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
