using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public override void StartTurn()
    {
        TurnComplete = false;
        StartCoroutine(SimulateTurn());
    }

    private IEnumerator SimulateTurn()
    {
        for (var i = 0; i < m_playerCards.Count; i++)
        {
            PlayCard(i);
            yield return new WaitForSeconds(0.5f);
        }
        
        EndTurn();
    }

    public override void PlayCard(int cardIndex)
    {
        if (CardCount > 0 && cardIndex >= 0 && cardIndex < CardCount)
        {
            Card cardToPlay = GetCard(cardIndex);
            if (cardToPlay.cost <= PlayerMana)
            {
                if(cardToPlay.type == CardType.Unit)
                    PlacerManager.instance.PlaceCard(this, cardToPlay, cardIndex,
                        new Vector3(Random.Range(-10, 10), 0, Random.Range(-10f, 10f)));
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
}
