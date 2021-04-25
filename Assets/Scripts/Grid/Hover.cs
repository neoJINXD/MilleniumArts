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
            
            // check if have unit on node.
			
            hoverMat = Grid.tileTrack[hoverNode.gridX, hoverNode.gridY].GetComponent<Renderer>();
            prevMat = hoverMat.material;
            hoverMat.material = hoveredTile;

            TurnManager.instance.hoveredTileText.text = "(" + hoverNode.gridX + "," + hoverNode.gridY + ")";
        }
    }

    void OnMouseExit()
    {
        TurnManager.instance.hoveredTileText.text = "";

        if (hoverMat != null)
            hoverMat.material = prevMat;
    }
}
