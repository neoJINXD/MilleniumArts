using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    private void Awake()
    {   
        cam = Camera.main;
    }

    void Update() 
    {
        transform.LookAt(cam.transform.position, -Vector3.up);
    }
}