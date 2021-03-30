using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;
    [SerializeField] private float panLimitX = 20f;
    [SerializeField] private float panLimitY = 20f;
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float minY = 20f;
    [SerializeField] private float maxY = 120f;



    private void Update() 
    {
        Vector3 position = transform.position;
        Vector3 movement = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            movement.z = 1f;
            // position.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            movement.z = -1f;
            // position.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            movement.x = 1f;
            // position.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x <= panBorderThickness)
        {
            movement.x = -1f;
            // position.x -= panSpeed * Time.deltaTime;
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
