using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class MovementManager : Singleton<MovementManager>
{
    private string unitTag = "Unit";
    private Unit unitSelected;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckDesiredUnit();
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
            }
        }
    }
}
