using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Leviathan_Eat", menuName = "Finite State Machines/FSM_Leviathan_Eat", order = 1)]
public class FSM_Leviathan_Eat : FiniteStateMachine
{
	/* Declare here, as attributes, all the variables that need to be shared among
	 * states and transitions and/or set in OnEnter or used in OnExit
	 * For instance: steering behaviours, blackboard, ...*/
	private Seek seek;
	private LEVIATHAN_Blackboard blackboard;
	private GameObject fish;
	private WanderAround wanderAround;
	private float eatTimer;


	public override void OnEnter()
	{
		/* Write here the FSM initialization code. This code is execute every time the FSM is entered.
		 * It's equivalent to the on enter action of any state
		 * Usually this code includes .GetComponent<...> invocations */
		seek = GetComponent<Seek>();
		blackboard = GetComponent<LEVIATHAN_Blackboard>();
		wanderAround = GetComponent<WanderAround>();
		base.OnEnter(); // do not remove
	}

	public override void OnExit()
	{
		/* Write here the FSM exiting code. This code is execute every time the FSM is exited.
		 * It's equivalent to the on exit action of any state
		 * Usually this code turns off behaviours that shouldn't be on when one the FSM has
		 * been exited. */
		base.DisableAllSteerings();
		base.OnExit();
	}

	public override void OnConstruction()
	{
		State wander = new State("Wander",
			() =>
			{
				blackboard.WanderTime = 0f;
				wanderAround.enabled = true;
			},
			() => { blackboard.WanderTime += Time.deltaTime; },
			() => { wanderAround.enabled = false; }
		);

		State huntFish = new State("Hunt_Fish",
			() =>
			{
				seek.target = fish;
				blackboard.speed = blackboard.speed * 2;
				seek.enabled = true;
			},
			() => { },
			() =>
			{
				wanderAround.enabled = false;
				blackboard.speed = blackboard.speed / 2;
				seek.enabled = false;
			});

		State eatFish = new State("Eat_Fish",
			() => { eatTimer = 0f; },
			() => { eatTimer += Time.deltaTime; },
			() => { Destroy(fish); }
		);

		/* STAGE 2: create the transitions with their logic(s)
		 * ---------------------------------------------------

		Transition varName = new Transition("TransitionName",
		    () => { }, // write the condition checkeing code in {}
		    () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
		);

		*/
		Transition hungry = new Transition("Hungry",
			() =>
			{
				fish = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "FISH", 1000f);
				return (fish != null && blackboard.WanderTime >= blackboard.WanderMaxTime);
			},
			() => { });

		Transition caughtFish = new Transition("Caught_Fish",
			() => { return SensingUtils.DistanceToTarget(gameObject, fish) < blackboard.eatRadius; },
			() => { });

		Transition stillHungry = new Transition("Still_Hungry",
			() =>
			{
				fish = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "FISH", 1000f);
				return (fish != null && eatTimer >= blackboard.eatMaxTimer && Random.value < 0.5f);
			},
			() => { });

		Transition notHungry = new Transition("Not_Hungry",
			() => { return eatTimer >= blackboard.eatMaxTimer; },
			() => { });
		/* STAGE 3: add states and transitions to the FSM
		 * ----------------------------------------------

		AddStates(...);

		AddTransition(sourceState, transition, destinationState);

		 */
		AddStates(wander, huntFish, eatFish);
		AddTransition(wander, hungry, huntFish);
		AddTransition(huntFish, caughtFish, eatFish);
		AddTransition(eatFish, stillHungry, huntFish);
		AddTransition(eatFish, notHungry, wander);


		/* STAGE 4: set the initial state

		initialState = ...

		 */
		initialState = wander;
	}
}