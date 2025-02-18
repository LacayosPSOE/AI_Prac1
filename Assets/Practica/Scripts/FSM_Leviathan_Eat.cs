using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Leviathan_Eat", menuName = "Finite State Machines/FSM_Leviathan_Eat", order = 1)]
public class FSM_Leviathan_Eat : FiniteStateMachine
{
	private Seek seek;
	private LEVIATHAN_Blackboard blackboard;
	private GameObject fish;
	private WanderAround wanderAround;
	private SteeringContext steeringContext;
	private float elapsedTime;


	public override void OnEnter()
	{
		seek = GetComponent<Seek>();
		blackboard = GetComponent<LEVIATHAN_Blackboard>();
		wanderAround = GetComponent<WanderAround>();
		steeringContext = GetComponent<SteeringContext>();
		base.OnEnter();
	}

	public override void OnExit()
	{
		base.DisableAllSteerings();
		base.OnExit();
	}

	public override void OnConstruction()
	{
		State wander = new State("Wander",
			() =>
			{
				elapsedTime = 0f;
				wanderAround.enabled = true;
			},
			() => { elapsedTime += Time.deltaTime; },
			() => { wanderAround.enabled = false; }
		);

		State huntFish = new State("Hunt_Fish",
			() =>
			{
				seek.target = fish;
				steeringContext.maxSpeed = blackboard.huntingSpeed;
				seek.enabled = true;
			},
			() => { },
			() =>
			{
				steeringContext.maxSpeed = blackboard.speed;
				seek.enabled = false;
			});

		State eatFish = new State("Eat_Fish",
			() => { elapsedTime = 0f; },
			() => { elapsedTime += Time.deltaTime; },
			() => { Destroy(fish); }
		);
		
		Transition hungry = new Transition("Hungry",
			() =>
			{
				fish = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "FISH", 1000f);
				return fish != null && elapsedTime < blackboard.WanderMaxTime;
			},
			() => { });

		Transition caughtFish = new Transition("Caught_Fish",
			() => { return fish && SensingUtils.DistanceToTarget(gameObject, fish) < blackboard.eatRadius; },
			() => { });

		Transition stillHungry = new Transition("Still_Hungry",
			() =>
			{
				fish = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "FISH", 1000f);
				return (fish != null && eatTimer >= blackboard.eatMaxTimer && Random.value < 0.5f);
			},
			() => { });

		Transition notHungry = new Transition("Not_Hungry",
			() => { return elapsedTime >= blackboard.eatMaxTimer; },
			() => { });

		AddStates(wander, huntFish, eatFish);
		AddTransition(wander, hungry, huntFish);
		AddTransition(huntFish, caughtFish, eatFish);
		AddTransition(eatFish, stillHungry, huntFish);
		AddTransition(eatFish, notHungry, wander);

		initialState = wander;
	}
}