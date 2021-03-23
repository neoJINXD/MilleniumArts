using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class MovementManager : Singleton<MovementManager>
{
    [SerializeField] private Material availablePosition;
    [SerializeField] private Material defaultMaterial;

    private string unitTag = "Unit";
    private Unit unitSelected;
    private bool hasSelected = false;
    private Pathfinding pathfinding;
    private int depth;
    private HashSet<Node> validMove;
    private Camera mainCam;
    private Ray ray;
    private RaycastHit hit;
    
    
    void Awake()
    {
        pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        mainCam = Camera.main;
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
        ray = mainCam.ScreenPointToRay(Input.mousePosition);

        ResetMaterial();
        
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
                
                DrawAvailable();
            }
        }
    }
    
    
    
    private void DrawAvailable()
    {
        if (unitSelected != null)
        {
            Vector3 initialPosition = unitSelected.transform.position;
            depth = pathfinding.depthLimit;
            validMove = pathfinding.BFSLimitSearch(new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), 
                false, depth);
    
            if (validMove != null && validMove.Count > 0)
            {
                foreach (var node in validMove)
                {
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = availablePosition;
                }
            }
        }
    }

    private void ResetMaterial()
    {
        if (validMove != null)
        {
            foreach (var node in validMove)
            {
                Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                newMat.material = defaultMaterial;
            }
        }
    }
    
    // For testing
    // private void OnDrawGizmos()
    // {
    //     if (unitSelected != null)
    //     {
    //         Vector3 initialPosition = unitSelected.transform.position;
    //         depth = pathfinding.depthLimit;
    //         var temp = pathfinding.BFSLimitSearch(new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), 
    //             false, depth);
    //
    //         if (temp != null && temp.Count > 0)
    //         {
    //             foreach (var node in temp)
    //             {
    //                 Gizmos.color = Color.blue;
    //                 Gizmos.DrawCube(node.worldPosition, Vector3.one * (1 - .1f));
    //             }
    //         }
    //     }
    // }
    
    // TESTING GetNodesMinMaxRange
    // private void DrawAvailable()
    // {
    //     if (unitSelected != null)
    //     {
    //         Vector3 initialPosition = unitSelected.transform.position;
    //         depth = pathfinding.depthLimit;
    //         validMove = pathfinding.GetNodesMinMaxRange(new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), 
    //             false, 4, 5);
    //
    //         if (validMove != null && validMove.Count > 0)
    //         {
    //             foreach (var node in validMove)
    //             {
    //                 Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
    //                 newMat.material = availablePosition;
    //             }
    //         }
    //     }
    // }
}
