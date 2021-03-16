using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;
    private string unitTag = "Unit";
    private Transform selection;
    private bool unitClicked = false;

    public bool checkIfClicked
    {
        get
        {
            return unitClicked;
        }
        set
        {
            unitClicked = value;
        }
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
    private void SetDefault()
    {
        if (selection != null)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            selection = null;
            
            unitClicked = false;
        }
    }

    private void SetSelected()
    {
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit))
         {
             var selected = hit.transform;

             if (selected.CompareTag(unitTag))
             {
                 var selectedRenderer = selected.GetComponent<Renderer>();

                 if (selectedRenderer != null)
                 {
                     selectedRenderer.material = selectedMaterial;
                 }
                 selection = selected;
                 
                 unitClicked = true;
             }
         }
    }
}
