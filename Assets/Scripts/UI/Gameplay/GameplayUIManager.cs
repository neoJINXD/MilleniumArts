using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private Player m_player;

    public int handCount;

    public GameObject defaultHandPanel;
    public GameObject dynamicHandPanel;

    public GameObject handSlot;

    public GameObject endTurnButton;

    public int hardCap;

    private bool dynamicHandFilled;

    private bool myTurn;

    // Start is called before the first frame update
    void Start()
    {
        handCount = 5;
        dynamicHandFilled = false;
        myTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        // used to test dynamic hand
        if (Input.GetKeyDown(KeyCode.Q))
        {
            handCount--;
            dynamicHandFilled = false;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            handCount++;
            dynamicHandFilled = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
            myTurn = true;

        if (Input.GetKeyDown(KeyCode.R))
            myTurn = false;


        if (handCount > 5)
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

        if (myTurn)
            endTurnButton.SetActive(true);
        else
            endTurnButton.SetActive(false);


    }

    void fillDynamicHand()
    {


        RectTransform handPanelRT = GameObject.Find("HandPanel").GetComponent<RectTransform>();

        foreach (Transform child in dynamicHandPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

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

    public void PlayCard(int cardIndex)
    {
        m_player.PlayCard(cardIndex);
    }
    
    public void endTurn()
    {
        myTurn = false;
        //m_player.EndTurn();
    }
}
