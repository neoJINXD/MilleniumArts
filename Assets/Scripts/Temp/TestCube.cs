using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    

    [SerializeField] float speed = 50;
    // private bool isSpin = false;

    private void FixedUpdate()
    {
        // if (isSpin)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + speed * Time.deltaTime, transform.rotation.eulerAngles.z);
    }

    // public void Enable()
    // {
    //     isSpin = true;
    // }
}
