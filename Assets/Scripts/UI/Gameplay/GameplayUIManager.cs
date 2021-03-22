using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    public int handCount;

    public GameObject defaultHandPanel;
    public GameObject dynamicHandPanel;

    public GameObject handSlot;

    public int hardCap;

    private bool dynamicHandFilled;

    // Start is called before the first frame update
    void Start()
    {
        handCount = 5;
        dynamicHandFilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // used to test dynamic hand
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (handCount > 4)
                handCount = 4;
            else
                handCount = 6;
        }

        if(handCount > 4)
        {
            defaultHandPanel.SetActive(false);
            dynamicHandPanel.SetActive(true);

            if(!dynamicHandFilled)
                fillDynamicHand();
        }
        else
        {
            defaultHandPanel.SetActive(true);
            dynamicHandPanel.SetActive(false);

            foreach (Transform child in dynamicHandPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            dynamicHandFilled = false;
        }

    }

    void fillDynamicHand()
    {
        handCount = 20;
        RectTransform handPanelRT = GameObject.Find("HandPanel").GetComponent<RectTransform>();
        float rectWidth = handPanelRT.rect.width;

        float initialX = 0;

        float finalX = (1.0f / (5) * rectWidth) * 4; // based on hand of 5 (5th card position)

        float xOffset = 1.0f / (handCount - 1) * finalX;

        for (int i = 0; i < handCount; i++)
        {
            GameObject go = Instantiate(handSlot, dynamicHandPanel.transform) as GameObject;
            float parentXOffset = go.transform.localPosition.x;
            go.transform.localPosition = new Vector3(initialX + parentXOffset, 0, 0);
            initialX += xOffset;
        }

        dynamicHandFilled = true;
    }
}
