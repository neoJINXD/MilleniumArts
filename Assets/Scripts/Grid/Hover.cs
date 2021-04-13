using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public Material hoveredTile;
    
    private Renderer hoverMat;
    private Renderer unhoverMat;
    private Material prevMat;
    
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
            prevMat = hoverMat.material;
            hoverMat.material = hoveredTile;
        }
    }

    void OnMouseExit()
    {
        if(hoverMat != null)
            hoverMat.material = prevMat;
    }
}
