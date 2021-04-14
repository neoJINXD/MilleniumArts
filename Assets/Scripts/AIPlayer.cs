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
	
	private GameObject cardHolder;

	private void Start()
	{
		m_pathfinding = GameObject.FindObjectOfType<Pathfinding>();
		
		if(!m_pathfinding)
			Debug.LogError("Pathfinding object not found!");
		
		cardHolder = new GameObject();
		cardHolder.transform.parent = transform;
		cardHolder.name = "AI card hand";
	}
	
    public override void StartTurn()
    {
        TurnComplete = false;
        StartCoroutine(SimulateTurn());
    }

    private IEnumerator SimulateTurn()
    {
		PickUpCards();
		
		yield return CheckKingCondition();

		if (PlayerMana > 0)
			yield return CheckCards();
		
		if (PlayerMana > 0)
			yield return AttackWithUnits();
		
        EndTurn();
    }
	
	private void PickUpCards()
	{
		m_playerCards.Clear();
		AddCard(GenerateCard());
		AddCard(GenerateCard());
		AddCard(GenerateCard());
		AddCard(GenerateCard());
		AddCard(GenerateCard());
	}
	
	private Card GenerateCard()
	{		
		TurnManager tm = TurnManager.instance;

		Card randomCard = tm.RandomCard();
		
		if(randomCard.GetType() == typeof(UnitCard))
		{
			UnitCard randomCardComponent = (UnitCard)cardHolder.AddComponent(randomCard.GetType());
			randomCardComponent.copyUnitCard((UnitCard)randomCard);
			return randomCardComponent;
		}
		else if(randomCard.GetType() == typeof(SpellCard))
		{
			SpellCard randomCardComponent = (SpellCard)cardHolder.AddComponent(randomCard.GetType());
			randomCardComponent.copySpellCard((SpellCard)randomCard);
			return randomCardComponent;
		}
		else
		{
			Debug.LogError("Card type error! : " + randomCard.GetType().ToString());
		}
		
		return new Card();
	}
	
	#region AI Actions
	
	private IEnumerator CheckKingCondition()
	{
		float kingHealthThreshold = 0.5f;
		if (m_behaviour == BehaviourType.Aggressive)
			kingHealthThreshold = 0.25f;
		else if (m_behaviour == BehaviourType.Defensive)
			kingHealthThreshold = 0.75f;
		
		if(King.GetCurrentHealth() / King.GetMaxHealth() < kingHealthThreshold)
		{
			List<Node> nearbyEnemyNodes = m_pathfinding.GetEnemyUnitNodesInRange(PlayerId, King.transform.position, 
				false, 1, 10);
			
			if(nearbyEnemyNodes.Count > 1)
			{
				// TODO: Move king away
				/*
				 * Select King
				 *		Move king away, depending on resources, how many tiles?
				 *		Could maybe do it last on the player AIs turn with it's remaining resources.
				 */
				
				// Bring closest ally to help
				Unit closestAlly = GetClosestAlly(King.transform.position);
				
				if(!closestAlly)
					yield break;
				
				if(closestAlly.unitType == Unit.UnitTypes.Priest)
				{
					if(CanHeal(closestAlly, King))
					{
						// TODO: Unit heal
						/*
						 * Check if can move healer unit in range of king to heal.
						 * If so, bring unit back in range:
						 *		Heal King
						 * Else
						 *      Bring closer to king for next turn. Or, we can make it so that the AI always keeps a healer in its spawn
						 */
					}
					else
					{
						yield return MoveUnit(closestAlly, King.transform.position);
					}
				}
				else
				{
					if(CanAttack(closestAlly, GetCloesetEnemy(King.transform.position)))
					{
						// TODO: Unit attack
						/*
						 * Move unit close to enemy unit near king.
						 * Attack enemy unit.
						 */
					}
					else
					{
						yield return MoveUnit(closestAlly, King.transform.position);
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
				// TODO: yield return PlayCard() 
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
				// TODO: unit cards need type
				// if ((UnitCard)card.unitType != Unit.UnitTypes.Priest)
				//	return true
			}
			
			// TODO: account for traps
		}
		
		if (m_behaviour == BehaviourType.Balanced)
		{
			// TODO determine behaviour, might be complex
			// TODO: account for traps
		}
		
		if (m_behaviour == BehaviourType.Defensive)
		{
			// TODO determine behaviour, might be complex
			// TODO: account for traps
		}
		
		return true;
	}

	private IEnumerator AttackWithUnits()
	{
		if (m_behaviour == BehaviourType.Aggressive)
		{
			// TODO
		}
		
		if (m_behaviour == BehaviourType.Balanced)
		{
			// TODO determine behaviour, might be complex
		}
		
		if (m_behaviour == BehaviourType.Defensive)
		{
			// TODO determine behaviour, might be complex
		}

		yield return null;
	}
	
	#endregion
	
	//Should be in Unit.cs
	private bool CanHeal(Unit currentUnit, Unit targetUnit)
	{
		// TODO
		return false;
	}
	
	//Should be in Unit.cs
	private bool CanAttack(Unit currentUnit, Unit targetUnit)
	{
		// TODO
		return false;
	}
	
	private IEnumerator MoveUnit(Unit unit, Vector3 targetLocation) //not sure if this needs to be a coroutine
	{
		// TODO: get and set unit path
		
		PathRequestManager.RequestPath(unit.transform.position, targetLocation, unit.GetCanFly(), unit.GetUnitPlayerID(), unit.OnPathFound, Pathfinding.Heuristic.TileDistance);
		//request path will create a coroutine itself and move the unit to the desired location accordingly 
		yield return null;
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
