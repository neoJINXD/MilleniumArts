using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Paning")]
    [SerializeField] bool enableKeyboardControls = true;
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] [Tooltip("How close to the edge will move the camera")] private float panBorderThickness = 10f;
    [SerializeField] private float panLimitX = 20f;
    [SerializeField] private float panLimitY = 20f;

    [Header("Zooming")]
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 120f;



    private void Update() 
    {
        Vector3 position = transform.position;
        Vector3 movement = new Vector3();

        if ( (enableKeyboardControls && Input.GetKey(KeyCode.W)) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            movement.z = 1f;
        }
        if ( (enableKeyboardControls && Input.GetKey(KeyCode.S)) || Input.mousePosition.y <= panBorderThickness)
        {
            movement.z = -1f;
        }
        if ( (enableKeyboardControls && Input.GetKey(KeyCode.D)) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            movement.x = 1f;
        }
        if ( (enableKeyboardControls && Input.GetKey(KeyCode.A)) || Input.mousePosition.x <= panBorderThickness)
        {
            movement.x = -1f;
        }

        position += movement * panSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, -panLimitX, panLimitX);
        position.z = Mathf.Clamp(position.z, -panLimitY, panLimitY);
        
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        position.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        position.y = Mathf.Clamp(position.y, minY, maxY);

        transform.position = position;
    }
}
