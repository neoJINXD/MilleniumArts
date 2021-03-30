using System.Collections.Generic;
using UnityEngine;

/*
    Abstract class to be inherited by a local player, networked player, client player?, and AI player
 */

public abstract class Player : MonoBehaviour
{
    [HideInInspector] public int PlayerId;
    public int PlayerMana;
    [SerializeField] protected List<Card> m_playerCards = new List<Card>();
    [SerializeField] protected List<Unit> m_playerUnits = new List<Unit>();
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
        if (CardCount > 0 && cardIndex >= 0 && cardIndex < CardCount)
        {
            Card cardToPlay = GetCard(cardIndex);
            if (cardToPlay.cost <= PlayerMana)
            {
                if(cardToPlay.type == CardType.Unit)
                    PlacerManager.instance.CreateUnit(this);
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
    
    public void RemoveCard(int index)
    {
        m_playerCards.RemoveAt(index);
    }

    public Card GetCard(int index)
    {
        return m_playerCards[index];
    }

    public void AddUnit(Unit unit)
    {
        m_playerUnits.Add(unit);
    }

    public int CardCount => m_playerCards.Count;
    public int UnitCount => m_playerUnits.Count;

    #endregion

}

