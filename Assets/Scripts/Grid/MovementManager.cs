using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class MovementManager : Singleton<MovementManager>
{
    private string unitTag = "Unit";
    private Unit unitSelected;
    private bool hasSelected = false;
    private Pathfinding pathfinding;
    private int depth;
    
    
    void Awake()
    {
        pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckDesiredUnit();
        }

        if (hasSelected)
        {
            unitSelected.SelectNewUnitPosition();
        }
    }
    
    private void CheckDesiredUnit()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selected = hit.transform;

            if (selected.CompareTag(unitTag))
            {
                if (unitSelected != null)
                {
                    unitSelected.isClicked = false;
                }
                
                unitSelected = selected.GetComponent<Unit>();
                unitSelected.isClicked = true;
                hasSelected = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (unitSelected != null)
        {
            Vector3 initialPosition = unitSelected.transform.position;
            depth = pathfinding.depthLimit;
            var temp = pathfinding.BFSLimitSearch(new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), 
                false, depth);

            if (temp != null && temp.Count > 0)
            {
                foreach (var node in temp)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (1 - .1f));
                }
            }
        }
    }
}
