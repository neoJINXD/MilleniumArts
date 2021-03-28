using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public override void StartTurn()
    {
        TurnComplete = false;
        for (var i = 0; i < m_playerCards.Count; i++)
        {
            PlayCard(i);
        }
        
        EndTurn();
    }

    public override void PlayCard(int cardIndex)
    {
        if (CardCount > 0 && cardIndex >= 0 && cardIndex < CardCount)
        {
            Card cardToPlay = GetCard(cardIndex);
            if (cardToPlay.ManaCost <= PlayerMana)
            {
                if(cardToPlay.Type == CardType.Unit)
                    m_placerManager.PlaceCard(this, cardToPlay, 
                        new Vector3(Random.Range(-10, 10), 0, Random.Range(-10f, 10f)));
            }
            else
            {
                Debug.LogWarning("Not enough mana");
            }
            
            RemoveCard(cardIndex);
        }
        else
        {
            Debug.LogError("Player out of cards or bad card index");
        }
    }
}
