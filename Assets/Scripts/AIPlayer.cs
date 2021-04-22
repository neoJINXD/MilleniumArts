using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
//using UnityEditor.Rendering;
using UnityEngine;

public class AIPlayer : Player
{
	
	[SerializeField] private GameObject attackAnimationHit;
	
	private GameObject animRef;
	
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

	private void Update()
	{
		if (animRef != null)
		{
			Destroy(animRef, animRef.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		}
	}
	
    public override void StartTurn()
    {
        TurnComplete = false;
        StartCoroutine(SimulateTurn());
    }

    private IEnumerator SimulateTurn()
    {
		PickUpCard();
		
		yield return CheckKingCondition();

		if (PlayerMana > 0)
			yield return CheckCards();
		
		if (PlayerMana > 0)
			yield return UseActiveUnits();

		yield return new WaitForSeconds(0.5f);
		
        EndTurn();
    }
	
	private void PickUpCard()
	{
		m_playerCards.Clear();
		Destroy(cardHolder);
		cardHolder = new GameObject();
		cardHolder.transform.parent = transform;
		cardHolder.name = "AI card hand";

		List<Card> cards = new List<Card>();
		cards.Add(GenerateCard());
		cards.Add(GenerateCard());
		cards.Add(GenerateCard());
		cards.Add(GenerateCard());
		cards.Add(GenerateCard());
		
		foreach (Card card in cards)
		{
			if (CardIsValued(card))
			{
				m_playerCards.Add(card);
				print("AI picked up " + card.name + " card");
				return;
			}
		}

		Debug.LogWarning("AI did not find any valuable cards");
		
		m_playerCards.Add(cards[Random.Range(0, cards.Count - 1)]);
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
		if (!ManaCheck(1) || !King)
			yield break;

		float kingHealthThreshold = 0.75f;
		if (m_behaviour == BehaviourType.Aggressive)
			kingHealthThreshold = 0.5f;
		else if (m_behaviour == BehaviourType.Defensive)
			kingHealthThreshold = 0.9f;
		
		if((float)King.GetCurrentHealth() / King.GetMaxHealth() < kingHealthThreshold)
		{
			List<Node> nearbyEnemyNodes = m_pathfinding.GetEnemyUnitNodesInRange(PlayerId, King.transform.position, 
				false, 0, 10);
			
			if(nearbyEnemyNodes.Count > 0)
			{
				Unit closestEnemy = GetClosetEnemy(King.transform.position);
				print("AI moving king away from closest enemy " + closestEnemy.name);
				if(closestEnemy)
					yield return MoveUnitAway(King, closestEnemy);
				
				if (!ManaCheck(1))
					yield break;
				
				// Bring closest ally to help
				Unit closestAlly = GetClosestAlly(King.transform.position);
				
				if(!closestAlly)
					yield break;
				
				if(closestAlly.unitType == Unit.UnitTypes.Priest)
				{
					if(CanHeal(closestAlly, King))
					{
						yield return Heal(closestAlly, King);
					}
					else
					{
						yield return MoveUnitTowards(closestAlly, King.transform.position);
						Debug.Log("AI moving " + closestAlly.name + " towards king to aheal");
					}
				}
				else
				{
					Unit enemy = GetClosetEnemy(King.transform.position);
					
					if (!enemy)
						yield break;
					
					if(CanAttack(closestAlly, enemy))
					{
						yield return Attack(closestAlly, enemy);
					}
					else
					{
						yield return MoveUnitTowards(closestAlly, King.transform.position);
						Debug.Log("AI moving " + closestAlly.name + " towards king to attack nearby enemies");
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
				List<Unit> otherUnits = GameLoop.instance.GetOtherPlayer(PlayerId).Units;
				
				if (playerCard.GetType() == typeof(UnitCard))
				{
					if (m_behaviour == BehaviourType.Aggressive)
					{
						if(m_playerCards.Count < otherUnits.Count + 2)
							PlayCard(playerCard);
					}
					
					if (m_behaviour == BehaviourType.Defensive)
					{
						if(m_playerCards.Count < otherUnits.Count + 4)
							PlayCard(playerCard);
					}
					
					if (m_behaviour == BehaviourType.Balanced)
					{
						if(m_playerCards.Count < otherUnits.Count + 3)
							PlayCard(playerCard);
					}
				}
				
				//TODO: add spell placement
				
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	private bool CardIsValued(Card card)
	{
		if (m_behaviour == BehaviourType.Aggressive)
		{
			if (m_playerUnits.Count > 4 && card.castType == CastType.OnEnemy)
				return true;

			if (card.GetType() == typeof(UnitCard))
			{
				if (((UnitCard)card).UnitType != Unit.UnitTypes.Priest)
					return true;
			}
		}
		
		if (m_behaviour == BehaviourType.Balanced)
		{
			if (m_playerUnits.Count > 4 && card.castType == CastType.OnEnemy)
				return true;

			if (card.GetType() == typeof(UnitCard))
			{
				return true;
			}
		}
		
		if (m_behaviour == BehaviourType.Defensive)
		{
			if (m_playerUnits.Count > 6 && card.castType == CastType.OnEnemy)
				return true;

			if (card.GetType() == typeof(UnitCard))
			{
				return true;
			}
		}
		
		return false;
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
					print("AI placed " + cardToPlay.name + " unit");
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
				
				if (!ally || !chosenUnit)
					yield break;
				
				if (ally && CanHeal(chosenUnit, ally) && ally.GetCurrentHealth() < ally.GetMaxHealth())
				{
					yield return Heal(chosenUnit, ally);
				}
				else
				{
					yield return MoveUnitTowards(chosenUnit, ally.transform.position);
					Debug.Log("AI unit moving " + chosenUnit.name + " to heal " + ally.name);
				}
			}
			else
			{
				Unit enemy = GetClosetEnemy(chosenUnit.transform.position);

				if (!enemy || !chosenUnit)
					yield break;
				
				if (enemy && CanAttack(chosenUnit, enemy))
				{
					yield return Attack(chosenUnit, enemy);
				}
				else
				{
					yield return MoveUnitTowards(chosenUnit, enemy.transform.position);
					Debug.Log("AI unit moving " + chosenUnit.name + " to attack " + enemy.name);
				}
			}

			yield return null;
		}
	}
	
	private IEnumerator Heal(Unit currentUnit, Unit targetUnit)
	{
		targetUnit.SetCurrentHealth(Mathf.Max(targetUnit.GetCurrentHealth() + targetUnit.GetDamage(), targetUnit.GetMaxHealth()));
		Debug.Log("<color=green>AI unit " + currentUnit.name + " healed " + targetUnit.name + "</color>");
		yield return new WaitForSeconds(0.4f);
		PlayerMana--;
	}

	private IEnumerator Attack(Unit currentUnit, Unit targetUnit)
	{
		targetUnit.SetCurrentHealth(targetUnit.GetCurrentHealth() - currentUnit.GetDamage());
		animRef = Instantiate(attackAnimationHit, currentUnit.transform, false);
		Debug.Log("<color=red>AI unit " + currentUnit.name + " attacked " + targetUnit.name + "</color>");
		PlayerMana--;
		yield return new WaitForSeconds(0.4f);
	}
	
	private IEnumerator MoveUnitTowards(Unit unit, Vector3 targetLocation)
	{
		Node closestNodeToTarget = null;
		float closetDist = Mathf.Infinity;
		Node[] movableNodes = Pathfinding.instance.GetNodesMinMaxRange(unit.transform.position, unit.GetCanFly(),
			1, (int)unit.GetMovementSpeed()).ToArray();
		
		if(movableNodes.Length < 1)
			yield break;

		for (int i = 0; i < movableNodes.Length; i++)
		{
			Node currentNode = movableNodes[i];
			float currentDist = Vector3.Distance(targetLocation, currentNode.worldPosition);
			if (currentDist < closetDist && currentNode.canWalkHere && !currentNode.unitInThisNode)
			{
				closestNodeToTarget = currentNode;
				closetDist = currentDist;
			}
		}

		yield return unit.AIFollowPath(Pathfinding.instance.AIFindPath(unit.transform.position,
			closestNodeToTarget.worldPosition, unit.GetCanFly(), unit.GetUnitPlayerID()));

		PlayerMana--;
	}

	private IEnumerator MoveUnitAway(Unit unit, Unit awayFrom)
	{
		Node farthestNodeFromUnit = null;
		float farthestDist = 0;
		Node[] movableNodes = Pathfinding.instance.GetNodesMinMaxRange(unit.transform.position, unit.GetCanFly(),
			1, (int)unit.GetMovementSpeed()).ToArray();
		
		if(movableNodes.Length < 1)
			yield break;

		for (int i = 0; i < movableNodes.Length; i++)
		{
			Node currentNode = movableNodes[i];
			float currentDist = Vector3.Distance(awayFrom.transform.position, currentNode.worldPosition);
			if (currentDist > farthestDist && currentNode.canWalkHere && !currentNode.unitInThisNode)
			{
				farthestNodeFromUnit = currentNode;
				farthestDist = currentDist;
			}
		}

		yield return unit.AIFollowPath(Pathfinding.instance.AIFindPath(unit.transform.position,
			farthestNodeFromUnit.worldPosition, unit.GetCanFly(), unit.GetUnitPlayerID()));

		PlayerMana--;
	}
	
	#endregion

	#region AIHelperMethods

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
		if (nearbyEnemies.Contains(m_pathfinding.gridRef.NodeFromWorldPoint(targetUnit.transform.position)))
		{
			return true; //return true if target's node is present, else false
		}
		else
		{
			return false;
		}
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
		List<Unit> enemyUnits = new List<Unit>(GameLoop.instance.GetOtherPlayer(PlayerId).Units);
		enemyUnits.Add(GameLoop.instance.GetOtherPlayer(PlayerId).King);
		
		Unit closestUnit = null;
		float shortestDist = Mathf.Infinity;
		foreach(Unit unit in enemyUnits)
		{
			if (unit == null)
				continue;
			
			float currentDist = Vector3.Distance(startPos, unit.transform.position);
			if(currentDist < shortestDist)
			{
				shortestDist = currentDist;
				closestUnit = unit;
			}
		}
		
		return closestUnit;
	}
	
	#endregion

}
