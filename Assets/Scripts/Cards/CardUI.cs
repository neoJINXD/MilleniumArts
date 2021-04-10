using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public UnitCard unitCard;
    public SpellCard spellCard;

    public GameObject unitCardPanel;
    public GameObject spellCardPanel;

    public Text cardCostText;
    public Text cardNameText;

    // unit card specific
    public Text cardHealthText;
    public Text cardDamageText;
    public Text cardDefenceText;
    public Text cardAccuracyText;
    public Text cardEvasionText;
    public Text cardMSText;
    public Text cardARText;
    public GameObject flyingPanel;

    // spell card specific

    public Text cardRangeText;
    public Text cardAOEText;
    public Text cardDescriptionText;

    void Awake()
    {
        if (this.GetComponent<UnitCard>() != null)
        {
            unitCard = this.GetComponent<UnitCard>();
            unitCardPanel = this.transform.GetChild(0).gameObject;

            unitCardPanel.SetActive(true);

            cardCostText = unitCardPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
            cardNameText = unitCardPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();

            cardHealthText = unitCardPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
            flyingPanel = unitCardPanel.transform.GetChild(3).transform.GetChild(0).gameObject;
            cardDamageText = unitCardPanel.transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            cardDefenceText = unitCardPanel.transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            cardAccuracyText = unitCardPanel.transform.GetChild(6).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            cardEvasionText = unitCardPanel.transform.GetChild(7).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            cardMSText = unitCardPanel.transform.GetChild(8).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
            cardARText = unitCardPanel.transform.GetChild(9).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
}
        else if (this.GetComponent<SpellCard>() != null)
        {
            spellCard = this.GetComponent<SpellCard>();
            spellCardPanel = this.transform.GetChild(1).gameObject;

            spellCardPanel.SetActive(true);
            cardCostText = spellCardPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
            cardNameText = spellCardPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();

            cardRangeText = unitCardPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
            cardAOEText = unitCardPanel.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>();
            cardDescriptionText = unitCardPanel.transform.GetChild(5).transform.GetChild(0).GetComponent<Text>();

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

        TurnManager.instance.currentTurnState = TurnManager.TurnState.Free;
        TurnManager.instance.loadPlayerHand();
    }
}
