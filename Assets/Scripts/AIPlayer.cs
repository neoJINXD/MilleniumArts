using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
	private enum BehaviourType
	{
		Aggressive,
		Balanced,
		Defensive
	}
	
	[SerializeField] private BehaviourType m_behaviour;
	private Pathfinding m_pathfinding;
		
	private void Start()
	{
		m_pathfinding = GameObject.FindObjectOfType<Pathfinding>();
			
		if(!m_pathfinding)
			Debug.LogError("Pathfinding object not found!");
	}
	
    public override void StartTurn()
    {
        TurnComplete = false;
        StartCoroutine(SimulateTurn());
    }

    private IEnumerator SimulateTurn()
    {
		yield return CheckKingCondition();

		if (PlayerMana > 0)
			yield return CheckCards();
		
		if (PlayerMana > 0)
			yield return AttackWithUnits();
		
        EndTurn();
    }
	
	#region AI Actions
	
	private IEnumerator CheckKingCondition()
	{
		float kingHealthThreshold = 0.5f;
		if (m_behaviour == BehaviourType.Aggressive)
			kingHealthThreshold = 0.25f;
		else if (m_behaviour == BehaviourType.Defensive)
			kingHealthThreshold = 0.75f;
		
		if(m_king.GetCurrentHealth() / m_king.GetMaxHealth() < kingHealthThreshold)
		{
			List<Node> nearbyEnemyNodes = m_pathfinding.GetEnemyUnitNodesInRange(PlayerId, m_king.transform.position, 
				false, 1, 10);
			
			if(nearbyEnemyNodes.Count > 1)
			{
				//TODO: Move king away
				
				//Bring closest ally to help
				Unit closestAlly = GetClosestAlly(m_king.transform.position);
				
				if(!closestAlly)
					yield break;
				
				if(closestAlly.unitType == Unit.UnitTypes.Priest)
				{
					if(CanHeal(closestAlly, m_king))
					{
						//TODO: Unit heal
					}
					else
					{
						yield return MoveUnit(closestAlly, m_king.transform.position);
					}
				}
				else
				{
					if(CanAttack(closestAlly, GetCloesetEnemy(m_king.transform.position)))
					{
						//TODO: Unit attack
					}
					else
					{
						yield return MoveUnit(closestAlly, m_king.transform.position);
					}
				}
				
			}
		}
	}

	private IEnumerator CheckCards()
	{
		foreach (Card playerCard in m_playerCards)
		{
			if (playerCard.cost < PlayerMana && CardIsValued(playerCard))
			{
				//TODO: yield return PlayCard() 
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	private bool CardIsValued(Card card)
	{
		if (m_behaviour == BehaviourType.Aggressive)
		{
			if (card.castType == CastType.OnEnemy)
				return true;

			if (card.GetType() == typeof(UnitCard))
			{
				//TODO: unit cards need type
				//if((UnitCard)card.unitType != Unit.UnitTypes.Priest)
				//	return true
			}
			
			//TODO: account for traps
		}
		
		if (m_behaviour == BehaviourType.Balanced)
		{
			//TODO determine behaviour, might be complex
			//TODO: account for traps
		}
		
		if (m_behaviour == BehaviourType.Defensive)
		{
			//TODO determine behaviour, might be complex
			//TODO: account for traps
		}
		
		return true;
	}

	private IEnumerator AttackWithUnits()
	{
		if (m_behaviour == BehaviourType.Aggressive)
		{
			//TODO
		}
		
		if (m_behaviour == BehaviourType.Balanced)
		{
			//TODO determine behaviour, might be complex
		}
		
		if (m_behaviour == BehaviourType.Defensive)
		{
			//TODO determine behaviour, might be complex
		}

		yield return null;
	}
	
	#endregion
	
	//Should be in Unit.cs
	private bool CanHeal(Unit currentUnit, Unit targetUnit)
	{
		//TODO
		return false;
	}
	
	//Should be in Unit.cs
	private bool CanAttack(Unit currentUnit, Unit targetUnit)
	{
		//TODO
		return false;
	}
	
	private IEnumerator MoveUnit(Unit unit, Vector3 targetLocation)
	{
		//TODO: get and set unit path
		
		yield return unit.FollowPath();
	}

	private Unit GetClosestAlly(Vector3 startPos)
	{		
		Unit closestUnit = null;
		float shortestDist = Mathf.Infinity;
		foreach(Unit unit in m_playerUnits)
		{
			float currentDist = Vector3.Distance(startPos, unit.transform.position);
			if(currentDist < shortestDist)
			{
				shortestDist = currentDist;
				closestUnit = unit;
			}
		}
		
		return closestUnit;
	}
	
	private Unit GetCloesetEnemy(Vector3 startPos)
	{
		List<Unit> enemyUnits = GameLoop.instance.GetOtherPlayer(PlayerId).Units;
		
		Unit closestUnit = null;
		float shortestDist = Mathf.Infinity;
		foreach(Unit unit in enemyUnits)
		{
			float currentDist = Vector3.Distance(startPos, unit.transform.position);
			if(currentDist < shortestDist)
			{
				shortestDist = currentDist;
				closestUnit = unit;
			}
		}
		
		return closestUnit;
	}

    public override void PlayCard(int cardIndex)
    {
        if (CardCount > 0 && cardIndex >= 0 && cardIndex < CardCount)
        {
            Card cardToPlay = GetCard(cardIndex);
            if (cardToPlay.cost <= PlayerMana)
            {
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
