using UnityEngine;
using System.Collections;
using UnityEditor;

public class Unit : MonoBehaviour {

    [SerializeField] private float speed = 20;
    [SerializeField] private bool canFly; //bool to toggle flying pathfinding
    Pathfinding.Heuristic Heuristic = Pathfinding.Heuristic.Dijkstra; //determine which heuristic to use
    Vector3[] path;
    int targetIndex;
    public bool isClicked = false;



    // dictionary of heap index and unit itself.

    /*
    For testing:
    public Transform target;
    void Start() 
    {
        // PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
    }*/
    
    // could maybe record units position here for pathfinding. this keyword to get that instance of unit. :)
    
    // if clicked, activate BFS.


    void Update()
    {
        SelectNewUnitPosition();
    }

    public void SelectNewUnitPosition()
    {
         if (Input.GetMouseButtonDown(0))
         {
              RaycastHit hit;
              Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                

              if (Physics.Raycast(ray, out hit, Mathf.Infinity))
              {
                  // fixes out of bounce error that occurs when unit selected.
                  if (Vector3.Distance(hit.point, transform.position) < 1)
                         return; // already at destination
                    
                  PathRequestManager.RequestPath(transform.position,hit.point, canFly, OnPathFound, Heuristic);
              }
         }
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() 
    {
        if (isClicked)
        {
            Vector3 currentWaypoint = path[0];
            while (true) {
                if (transform.position == currentWaypoint) {
                    targetIndex++;
                    if (targetIndex >= path.Length) {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
            
                yield return null;
            }
        }
    }

    public void OnDrawGizmos() 
    {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * (1f - 0.3f));

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}