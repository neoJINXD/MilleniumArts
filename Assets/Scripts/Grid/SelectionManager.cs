using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class SelectionManager : Singleton<SelectionManager>
{
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material greyUnwalkableMaterial;
    [SerializeField] private Material defaultUnwalkableMaterial;
    private string unitTag = "Unit";
    private Transform selection;
    private Renderer selectedRenderer;
    private GameObject[] obstacles;

    private void Awake()
    {
        obstacles = GameObject.FindGameObjectsWithTag("Unwalkable");
    }
    
    private void Update()
    {
        // SetDefault();

        if (Input.GetMouseButtonDown(0))
        {
            SetSelected();
        }
    }

    // sets back to original color, idea is to call this after it reached it's path.
    private void ResetSelected()
    {
        if (selection != null)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            selection = null;
        }
    }
    
    private void SetSelected()
    {
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;
         
         DefaultObstacleColor();

         if (Physics.Raycast(ray, out hit))
         {
             var selected = hit.transform;

             if (selected.CompareTag(unitTag))
             {
                 ResetSelected();

                 SetObstaclesGrey();
                 
                 selectedRenderer = selected.GetComponent<Renderer>();

                 if (selectedRenderer != null)
                 {
                     selectedRenderer.material = selectedMaterial;
                 }
                 selection = selected;
             }
         }
    }
    

    private void SetObstaclesGrey()
    {
        foreach (GameObject o in obstacles)
            o.GetComponent<Renderer>().material = greyUnwalkableMaterial;
    }

    private void DefaultObstacleColor()
    {
        foreach (GameObject o in obstacles)
            o.GetComponent<Renderer>().material = defaultUnwalkableMaterial;
    }
}
