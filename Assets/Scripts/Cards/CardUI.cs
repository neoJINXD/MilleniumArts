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

            bool playerOne= true; // toggle this appropriately

            if(playerOne)
            {
                if (unitCard.UnitType == Unit.UnitTypes.Soldier)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Soldier_Blue");
                else if (unitCard.UnitType == Unit.UnitTypes.Knight)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Knight_Blue");
                else if (unitCard.UnitType == Unit.UnitTypes.Assassin)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Assassin_Blue");
                else if (unitCard.UnitType == Unit.UnitTypes.Priest)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Priest_Blue");
                else if (unitCard.UnitType == Unit.UnitTypes.Archer)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Archer_Blue");
                else if (unitCard.UnitType == Unit.UnitTypes.DragonRider)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/DragonRider_Blue");
            }
            else
            {
                if (unitCard.UnitType == Unit.UnitTypes.Soldier)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Soldier_Red");
                else if (unitCard.UnitType == Unit.UnitTypes.Knight)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Knight_Red");
                else if (unitCard.UnitType == Unit.UnitTypes.Assassin)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Assassin_Red");
                else if (unitCard.UnitType == Unit.UnitTypes.Priest)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Priest_Red");
                else if (unitCard.UnitType == Unit.UnitTypes.Archer)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/Archer_Red");
                else if (unitCard.UnitType == Unit.UnitTypes.DragonRider)
                    unitCardPanel.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Unit/DragonRider_Red");
            }
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

            cardCostText.text = "" + spellCard.cost;
            cardNameText.text = "" + spellCard.name;

            cardRangeText.text = "" + spellCard.minRange + " - " + spellCard.maxRange;
            cardAOEText.text = "" + spellCard.aoeMinRange + " - " + spellCard.aoeMaxRange;
            cardDescriptionText.text = "" + spellCard.description;

            if (spellCard.id == 6)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Smite");
            else if (spellCard.id == 7)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Snipe");
            else if (spellCard.id == 8)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/HeavenlySmite");
            else if (spellCard.id == 9)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Prayer");
            else if (spellCard.id == 10)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Vitality");
            else if (spellCard.id == 11)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Endurance");
            else if (spellCard.id == 12)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Vigor");
            else if (spellCard.id == 13)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Nimbleness");
            else if (spellCard.id == 14)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Agility");
            else if (spellCard.id == 15)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Precision");
            else if (spellCard.id == 16)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Oracle");
            else if (spellCard.id == 17)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/DisarmTrap");
            else if (spellCard.id == 18)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Provisions");
            else if (spellCard.id == 19)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Reinforcements");
            else if (spellCard.id == 20)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Greed");
            else if (spellCard.id == 21)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Warcry");
            else if (spellCard.id == 22)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Rebirth");
            else if (spellCard.id == 23)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/Assassinate");
            else if (spellCard.id == 24)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/BearTrap");
            else if (spellCard.id == 25)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/LandMine");
            else if (spellCard.id == 26)
                spellCardPanel.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardImages/Active/RoyalPledge");
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
        Destroy(GameplayUIManager.instance.cardZoomInPanel.transform.GetChild(0).gameObject);
        TurnManager.instance.loadPlayerHand();
    }
}
