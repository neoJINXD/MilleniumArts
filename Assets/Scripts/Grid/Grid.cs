using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To test this scrip, run the game in scene view and move the map game object around.
 * You will see the clip movement between grids of the unit.
 */
public class Grid : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform unit;
    
    // the size of the grid
    [SerializeField] private float size;

    private Vector3 newPosition;

    void LateUpdate()
    {
        newPosition.x = Mathf.Floor(target.position.x / size) * size;
        newPosition.y = Mathf.Floor(target.position.y / size) * size;
        newPosition.z = Mathf.Floor(target.position.z / size) * size;

        unit.transform.position = newPosition;
    }
    
}
