using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public UnitCard unitCard;
    public SpellCard spellCard;

    public GameObject unitCardPanel;
    public GameObject spellCardPanel;

    public TextMeshProUGUI cardCostText;
    public TextMeshProUGUI cardNameText;

    // unit card specific
    public TextMeshProUGUI cardHealthText;
    public TextMeshProUGUI cardDamageText;
    public TextMeshProUGUI cardDefenceText;
    public TextMeshProUGUI cardAccuracyText;
    public TextMeshProUGUI cardEvasionText;
    public TextMeshProUGUI cardMSText;
    public TextMeshProUGUI cardARText;
    public GameObject flyingPanel;

    // spell card specific

    public TextMeshProUGUI cardRangeText;
    public TextMeshProUGUI cardAOEText;
    public TextMeshProUGUI cardDescriptionText;

    void Awake()
    {
        if (this.GetComponent<UnitCard>() != null)
        {
            unitCard = this.GetComponent<UnitCard>();
            unitCardPanel = this.transform.GetChild(0).gameObject;

            unitCardPanel.SetActive(true);

            cardCostText = unitCardPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardNameText = unitCardPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            cardHealthText = unitCardPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            flyingPanel = unitCardPanel.transform.GetChild(3).transform.GetChild(0).gameObject;
            cardDamageText = unitCardPanel.transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardDefenceText = unitCardPanel.transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardAccuracyText = unitCardPanel.transform.GetChild(6).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardEvasionText = unitCardPanel.transform.GetChild(7).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardMSText = unitCardPanel.transform.GetChild(8).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardARText = unitCardPanel.transform.GetChild(9).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
}
        else if (this.GetComponent<SpellCard>() != null)
        {
            spellCard = this.GetComponent<SpellCard>();
            spellCardPanel = this.transform.GetChild(1).gameObject;

            spellCardPanel.SetActive(true);
            cardCostText = spellCardPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardNameText = spellCardPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            cardRangeText = spellCardPanel.transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardAOEText = spellCardPanel.transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            cardDescriptionText = spellCardPanel.transform.GetChild(5).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (unitCard != null)
        {
            unitCardPanel.SetActive(true);

            cardCostText.text = "" + unitCard.cost;
            cardNameText.text = "" + unitCard.name;

            cardHealthText.text = "" + unitCard.health;
            cardDamageText.text = "" + unitCard.damage;
            cardDefenceText.text = "" + unitCard.defence;
            cardARText.text = "" + unitCard.minAttackRange + " - " + unitCard.maxAttackRange;
            cardAccuracyText.text = "" + unitCard.accuracy;
            cardEvasionText.text = "" + unitCard.evasion;
            cardMSText.text = "" + unitCard.moveSpeed;

            if (unitCard.flying)
                flyingPanel.SetActive(true);
        }
        else if (spellCard != null)
        {
            spellCardPanel.SetActive(true);

            cardCostText.text = "" + spellCard.cost;
            cardNameText.text = "" + spellCard.name;

            cardRangeText.text = "" + spellCard.minRange + " - " + spellCard.maxRange;
            cardAOEText.text = "" + spellCard.aoeMinRange + " - " + spellCard.aoeMaxRange;
            cardDescriptionText.text = "" + spellCard.description;
        }
    }

    public void addClickedCardToHand()
    {
        Player currentPlayer = GameLoop.instance.GetCurrentPlayer();
        if (unitCard != null)
            currentPlayer.AddCard(GetComponent<UnitCard>());
        else if (spellCard != null)
            currentPlayer.AddCard(GetComponent<SpellCard>());

        TurnManager.instance.cardDrawPanel.SetActive(false);
        TurnManager.instance.currentTurnState = TurnManager.TurnState.Free;
        
        GameObject child = GameplayUIManager.instance.cardZoomInPanel.transform.GetChild(0).gameObject;
        if (child)
            Destroy(GameplayUIManager.instance.cardZoomInPanel.transform.GetChild(0).gameObject);
                
        TurnManager.instance.loadPlayerHand();
    }
}
