using System.Collections.Generic;
using UnityEngine;

/*
    Abstract class to be inherited by a local player, networked player, client player?, and AI player
 */

public abstract class Player : MonoBehaviour
{
    [HideInInspector] public int PlayerId;
    public int PlayerMana;
    public int PlayerMaxMana;
    public bool KingAlive = true;
	public Unit King;
    [SerializeField] protected List<Card> m_playerCards = new List<Card>();
    [SerializeField] protected List<Unit> m_playerUnits = new List<Unit>();
	public List<Unit> Units => m_playerUnits;
    public bool TurnComplete { get; protected set; }

    public Player()
    {
        TurnComplete = true;
    }

    public virtual void StartTurn()
    {
        TurnComplete = false;
    }
    
    public virtual void EndTurn()
    {
        TurnComplete = true;

        foreach(Unit unit in m_playerUnits)
        {
            unit.SetMovementSpeedLeft(unit.GetMovementSpeed());
            unit.SetCanAttack(true);
        }
    }

    #region Unit & Card Setters/Getters

    public virtual void PlayCard(Card cardToPlay)
    {
        if (CardCount > 0)
        {
            if (cardToPlay.cost <= PlayerMana)
            {
                if(cardToPlay.GetType() == typeof(UnitCard))
                {
                }

                PlayerMana -= cardToPlay.cost;
                RemoveCard(cardToPlay);
            }
            else
            {
                Debug.LogWarning("Not enough mana");
            }

        }
        else
        {
            Debug.LogError("Player out of cards or bad card index");
        }
    }

    public bool ManaCheck(int cost)
    {
        bool canUse = PlayerMana >= cost;
        if(!canUse)
            GameplayUIManager.instance.NotEnoughMana();

        return canUse;
    }

    public void RemoveCard(Card card)
    {
        m_playerCards.RemoveAt(card.indexInHand);

        for(int x = 0; x < m_playerCards.Count; x++)
            m_playerCards[x].indexInHand = x;
    }

    public Card GetCard(int index)
    {
        return m_playerCards[index];
    }

    public List<Card> GetHand()
    {
        return m_playerCards;
    }

    public void AddCard(Card card)
    {
        UnitCard unitCopyCard = null;
        SpellCard spellCopyCard = null;

        if (card is UnitCard)
        {
            unitCopyCard = new UnitCard();
            unitCopyCard.copyUnitCard((UnitCard)card);
            m_playerCards.Add(unitCopyCard);
        }
        else if (card is SpellCard)
        {
            spellCopyCard = new SpellCard();
            spellCopyCard.copySpellCard((SpellCard)card);
            m_playerCards.Add(spellCopyCard);
        }

        m_playerCards[m_playerCards.Count - 1].indexInHand = m_playerCards.Count - 1;

    }

    public void AddUnit(Unit unit)
    {
        m_playerUnits.Add(unit);
    }
    
    public void RemoveUnit(Unit unit)
    {
        m_playerUnits.Remove(unit);
    }

    public int CardCount => m_playerCards.Count;
    public int UnitCount => m_playerUnits.Count;

    public void DrawCard()
    {
        TurnManager.instance.ShowCardSelection();
    }

    #endregion

}
