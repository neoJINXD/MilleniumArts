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

    private RectTransform handPanelRT;
    private Animator animator;
    private static readonly int k_notEnoughMana = Animator.StringToHash("NotEnoughMana");

    public GameObject cardZoomInPanel;
    public GameObject cardPrefab;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        handCount = 5;
        dynamicHandFilled = false;

        handPanelRT = GameObject.Find("HandPanel").GetComponent<RectTransform>();
    }

    void Update()
    {
        if(!GameLoop.instance.GameOver)
        {
            if (GameManager.instance.networked)
            {
                if (Photon.Pun.PhotonNetwork.IsMasterClient)
                    endTurnButton.SetActive(((NetworkedPlayer)GameLoop.instance.GetCurrentPlayer()).amIP1);
                if (!Photon.Pun.PhotonNetwork.IsMasterClient)
                    endTurnButton.SetActive(!((NetworkedPlayer)GameLoop.instance.GetCurrentPlayer()).amIP1);
            }
            else
                endTurnButton.SetActive(GameLoop.instance.GetCurrentPlayer().PlayerId == 0);
        }
    }

    public void NotEnoughMana()
    {
        animator.SetTrigger(k_notEnoughMana);
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

    public void endTurn()
    {
        TurnManager.instance.updateTurnUpdate("");

        if (GameManager.instance.networked)
            GameManager.instance.view.RPC("EndCurrentPlayerTurn", Photon.Pun.RpcTarget.All);
        else
            GameLoop.instance.EndCurrentPlayer();

        TurnManager.instance.cardDrawPanel.SetActive(false);
    }
}
