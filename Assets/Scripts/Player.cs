using System.Collections.Generic;
using UnityEngine;

/*
    Abstract class to be inherited by a local player, networked player, client player?, and AI player
 */

public abstract class Player : MonoBehaviour
{
    [HideInInspector] public int PlayerId;
    public int PlayerMana;
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
    }

    #region Unit & Card Setters/Getters

    public virtual void PlayCard(int cardIndex)
    {
        // temporary changed this to simulate placing enemy unit for testing
        // expecting to change some stuff to accomodate AI or networking
        // - rey

        TurnManager.instance.placingEnemyUnit = true;
        TurnManager.instance.currentPlayer = this;

        /*if (CardCount > 0 && cardIndex >= 0 && cardIndex < CardCount)
        {
            Card cardToPlay = GetCard(cardIndex);
            if (cardToPlay.cost <= PlayerMana)
            {
                if(cardToPlay.type == CardType.Unit)
                {
                    TurnManager.instance.placingEnemyUnit = true;
                    TurnManager.instance.currentPlayer = this;
                }

            }
            else
            {
                Debug.LogWarning("Not enough mana");
            }

        }
        else
        {
            Debug.LogError("Player out of cards or bad card index");
        }*/
    }
    
    public void RemoveCard(int index)
    {
        m_playerCards.RemoveAt(index);
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
        m_playerCards.Add(card);
        m_playerCards[m_playerCards.Count - 1].indexInHand = m_playerCards.Count - 1;
        print(m_playerCards[m_playerCards.Count - 1].indexInHand + "DXDXD");
    }

    public void AddUnit(Unit unit)
    {
        m_playerUnits.Add(unit);
    }

    public int CardCount => m_playerCards.Count;
    public int UnitCount => m_playerUnits.Count;

    public void DrawCard()
    {
        TurnManager.instance.ShowCardSelection();
    }

    #endregion

}
