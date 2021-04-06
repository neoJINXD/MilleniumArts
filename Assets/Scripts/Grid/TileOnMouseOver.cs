using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileOnMouseOver : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Material AOEMaterial;

    private Pathfinding pf;
    private Grid grid;
    private TurnManager tm;
    private HashSet<Node> availableNodes;
    private HashSet<Node> aoeNodes;
    private bool hovered;
    private Card storedCard;

    // Start is called before the first frame update
    void Awake()
    {
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
        grid = GameObject.FindWithTag("Pathfinding").GetComponent<Grid>();
        tm = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        availableNodes = tm.selectableNodes;
        storedCard = tm.storedCard;
        defaultMaterial = tm.defaultMaterial;
        targetMaterial = tm.targetMaterial;
        AOEMaterial = tm.AOEMaterial;
        hovered = false;
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnMouseOver()
    {
        if(availableNodes != null)
        {
            aoeNodes = pf.GetNodesMinMaxRange(this.transform.position, false, storedCard.minRange, storedCard.maxRange);
            availableNodes = tm.selectableNodes;
            if (availableNodes.Contains(grid.NodeFromWorldPoint(this.transform.position)))
            {
                pf.minDepthLimit = storedCard.aoeMinRange;
                pf.maxDepthLimit = storedCard.aoeMaxRange;

                aoeNodes = pf.GetNodesMinMaxRange(this.transform.position, false, storedCard.aoeMinRange, storedCard.aoeMaxRange);
                foreach (Node node in aoeNodes)
                {
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = AOEMaterial;
                }

                hovered = true;
            }
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnMouseExit()
    {
        if(hovered)
        {
            foreach (Node node in aoeNodes)
            {
                if(availableNodes.Contains(node))
                {
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = targetMaterial;
                }
                else
                {
                    Renderer newMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
                    newMat.material = defaultMaterial;
                }

                hovered = false;
            }
        }
    }
}