using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private Material hoveredTile;
    [SerializeField] private Material defaultMat;
    
    private Renderer hoverMat;
    private Renderer unhoverMat;
    private Grid gridRef;
    private RaycastHit hit;
    

    private void Awake()
    {
        gridRef = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();
    }
    
    void OnMouseEnter()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Transform pos = hit.transform;

            Node hoverNode = gridRef.NodeFromWorldPoint(new Vector3(pos.position.x, pos.position.y, pos.position.z));
			
            hoverMat = Grid.tileTrack[hoverNode.gridX, hoverNode.gridY].GetComponent<Renderer>();
            hoverMat.material = hoveredTile;
        }
    }

    void OnMouseExit()
    {
        hoverMat.material = defaultMat;
    }
}
