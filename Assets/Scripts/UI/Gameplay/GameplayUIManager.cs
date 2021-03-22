using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    public int handCount;
    public GameObject defaultHandPanel;
    public GameObject dynamicHandPanel;

    public int hardCap;

    // Start is called before the first frame update
    void Start()
    {
        handCount = 5;
    }

    // Update is called once per frame
    void Update()
    {
        // used to test dynamic hand
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (handCount > 5)
                handCount = 5;
            else
                handCount = 6;
        }

        if(handCount > 5)
        {
            defaultHandPanel.SetActive(false);
            dynamicHandPanel.SetActive(true);
        }
        else
        {
            defaultHandPanel.SetActive(true);
            dynamicHandPanel.SetActive(false);
        }
    }
}
