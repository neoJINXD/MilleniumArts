using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

public class GameplayUIManager: Singleton<GameplayUIManager>
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

    private RectTransform handPanelRT;

    public GameObject cardZoomInPanel;
    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        handCount = 5;
        dynamicHandFilled = false;
        myTurn = true;

        handPanelRT = GameObject.Find("HandPanel").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTurn)
            endTurnButton.SetActive(true);
        else
            endTurnButton.SetActive(false);


    }

    public void fillDynamicHand()
    {
        int x = 0;

        GameObject[] children = new GameObject[dynamicHandPanel.transform.childCount];

        foreach (Transform child in dynamicHandPanel.transform)
        {
            children[x] = child.gameObject;
            x += 1;
        }


        foreach (GameObject child in children)
            GameObject.DestroyImmediate(child);


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
        GameLoop.instance.EndCurrentPlayer();
    }
}
