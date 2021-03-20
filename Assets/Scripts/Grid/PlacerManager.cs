using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class PlacerManager : Singleton<PlacerManager>
{
    [SerializeField] private GameObject unitCreation;
    private bool placerClicked = false;
    private const float lockAxis = 27f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && placerClicked)
        {
            Vector3 areaToInstantiate = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lockAxis));
            Instantiate(unitCreation, areaToInstantiate, Quaternion.identity);
            placerClicked = false;
        }
    }
    public void CreateUnit()
    {
        placerClicked = true;
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
