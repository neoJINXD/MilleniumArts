using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor.Rendering;
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
			yield return UseActiveUnits();
		
        EndTurn();
    }
	
	private void PickUpCards()
	{
		m_playerCards.Clear();
		Destroy(cardHolder);
		cardHolder = new GameObject();
		cardHolder.transform.parent = transform;
		cardHolder.name = "AI card hand";
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
				// move king away from closest enemy unit.
				// need to figure out a way to cap depending on resources.
				King.transform.position = nearbyEnemyNodes[0].worldPosition - King.transform.position;

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
					if(CanAttack(closestAlly, GetClosetEnemy(King.transform.position)))
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
		for (var i = 0; i < m_playerCards.Count; i++)
		{
			Card playerCard = m_playerCards[i];
			if (playerCard.cost < PlayerMana && CardIsValued(playerCard))
			{
				PlayCard(playerCard); 
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	private bool CardIsValued(Card card)
	{
		return true;
		
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

	public override void PlayCard(Card cardToPlay)
	{
		if (CardCount > 0)
		{
			if (cardToPlay.cost <= PlayerMana)
			{
				if (cardToPlay.GetType() == typeof(SpellCard))
				{
            
				}
				else if (cardToPlay.GetType() == typeof(UnitCard))
				{
					Node[] placeableNodes = Grid.instance.GetPlaceableNodes(cardToPlay).ToArray();
					Node nodeToPlaceIn = placeableNodes[Random.Range(0, placeableNodes.Length - 1)];
					CardEffectManager.instance.CreateUnit(((UnitCard)cardToPlay).UnitType, nodeToPlaceIn);
					RemoveCard(cardToPlay);
					PlayerMana -= cardToPlay.cost;
					print("AI played a unit");
					TurnManager.instance.cardSuccessful = false;
				}
				else
				{
					Debug.LogError("Bad card type");
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
		}
	}
	
	#endregion

	private IEnumerator UseActiveUnits()
	{
		if (m_playerUnits.Count < 1)
			yield break;

		while (PlayerMana > 0)
		{
			Unit chosenUnit = m_playerUnits[Random.Range(0, m_playerUnits.Count - 1)]; //TODO: make not random
		
			if (m_behaviour == BehaviourType.Aggressive)
			{
				// TODO: choose unit with the most potential for damage (including distance to nearest enemies)
				// if (CanAttack(chosenUnit, nearestEnemy)
				//		yield return AttackWithUnit();
			}
		
			if (m_behaviour == BehaviourType.Balanced)
			{
				// TODO random?
			}
		
			if (m_behaviour == BehaviourType.Defensive)
			{
				// TODO: ??
			}

			if (chosenUnit.unitType == Unit.UnitTypes.Priest)
			{
				Unit ally = GetAllyInNeed();
				
				if (!ally)
					yield break;
				
				if (ally && CanHeal(chosenUnit, ally) && ally.GetCurrentHealth() < ally.GetMaxHealth())
				{
					ally.SetCurrentHealth(Mathf.Max(ally.GetCurrentHealth() + chosenUnit.GetDamage(), ally.GetMaxHealth()));
					PlayerMana--;
				}
				else
				{
					yield return MoveUnit(chosenUnit, ally.transform.position);
					PlayerMana--;
				}
			}
			else
			{
				Unit enemy = GetClosetEnemy(chosenUnit.transform.position);

				if (!enemy)
					yield break;
				
				if (enemy && CanAttack(chosenUnit, enemy))
				{
					enemy.SetCurrentHealth(enemy.GetCurrentHealth() - chosenUnit.GetDamage());
					PlayerMana--;
				}
				else
				{
					yield return MoveUnit(chosenUnit, enemy.transform.position);
					PlayerMana--;
				}
			}

			yield return null;
		}
	}
	
	//Should be in Unit.cs
	private bool CanHeal(Unit currentUnit, Unit targetUnit)
	{
		List<Node> nearbyAllies = m_pathfinding.GetAllyUnitNodesInRange(currentUnit.GetUnitPlayerID(),
			currentUnit.transform.position, currentUnit.GetCanFly(), currentUnit.GetMinRange(),
			currentUnit.GetMaxRange());
		//Gets a list of nodes of nearby allies and searches the target's node in this list
		if (nearbyAllies.Contains(m_pathfinding.gridRef.NodeFromWorldPoint(targetUnit.transform.position)))
		{
			return true; //return true if target's node is present, else false
		}
		else
		{
			return false;
		}
	}
	
	//Should be in Unit.cs
	private bool CanAttack(Unit currentUnit, Unit targetUnit)
	{
		List<Node> nearbyEnemies = m_pathfinding.GetEnemyUnitNodesInRange(currentUnit.GetUnitPlayerID(),
			currentUnit.transform.position, currentUnit.GetCanFly(), currentUnit.GetMinRange(),
			currentUnit.GetMaxRange());
		//Gets a list of nodes of nearby enemies and searches the target's node in this list
		if (nearbyEnemies.Contains(m_pathfinding.gridRef.NodeFromWorldPoint(currentUnit.transform.position)))
		{
			return true; //return true if target's node is present, else false
		}
		else
		{
			return false;
		}
	}
	
	private IEnumerator MoveUnit(Unit unit, Vector3 targetLocation)
	{
		Node closestNodeToTarget = null;
		float closetDist = Mathf.Infinity;
		Node[] movableNodes = Pathfinding.instance.GetNodesMinMaxRange(unit.transform.position, unit.GetCanFly(),
			unit.GetMinRange(), unit.GetMaxRange()).ToArray();
		
		if(movableNodes.Length < 1)
			yield break;

		for (int i = 0; i < movableNodes.Length; i++)
		{
			float currentDist = Vector3.Distance(targetLocation, movableNodes[i].worldPosition);
			if (currentDist < closetDist)
			{
				closestNodeToTarget = movableNodes[i];
				closetDist = currentDist;
			}
		}

		yield return unit.AIFollowPath(Pathfinding.instance.AIFindPath(unit.transform.position,
			closestNodeToTarget.worldPosition, unit.GetCanFly(), unit.GetUnitPlayerID(), unit.GetMinRange(), unit.GetMaxRange()));

		PlayerMana--;
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
	
	private Unit GetAllyInNeed()
	{		
		Unit allyInNeed = null;
		int leastHealth = int.MaxValue;
		foreach(Unit unit in m_playerUnits)
		{
			int currentHealth = unit.GetCurrentHealth();
			if(currentHealth < leastHealth && currentHealth < unit.GetMaxHealth())
			{
				leastHealth = currentHealth;
				allyInNeed = unit;
			}
		}
		
		return allyInNeed;
	}
	
	private Unit GetClosetEnemy(Vector3 startPos)
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
	
}
