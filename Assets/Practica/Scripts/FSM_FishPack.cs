using FSMs;
using UnityEngine;
using Steerings;
using Unity.VisualScripting;
using State = FSMs.State;

[CreateAssetMenu(fileName = "FSM_FishPack", menuName = "Finite State Machines/FSM_FishPack", order = 1)]
public class FSM_FishPack : FiniteStateMachine
{
	/* Declare here, as attributes, all the variables that need to be shared among
	 * states and transitions and/or set in OnEnter or used in OnExit
	 * For instance: steering behaviours, blackboard, ...*/
	private FISH_blackboard blackboard;
	private FleePlusOA flee;
	private VelocityMatching velocityMatch;
	private Arrive arrive;
	private SteeringContext steeringContext;

	public override void OnEnter()
	{
		/* Write here the FSM initialization code. This code is execute every time the FSM is entered.
		 * It's equivalent to the on enter action of any state
		 * Usually this code includes .GetComponent<...> invocations */
		blackboard = GetComponent<FISH_blackboard>();
		flee = GetComponent<FleePlusOA>();
		velocityMatch = GetComponent<VelocityMatching>();
		arrive = GetComponent<Arrive>();
		steeringContext = GetComponent<SteeringContext>();
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
		State wanderInGroup = new State("Wander",
			() =>
			{
				velocityMatch.target = blackboard.fishAttractor;
				velocityMatch.enabled = true;
			},
			() => { },
			() => { velocityMatch.enabled = false; }
		);

		State fleeFromMonster = new State("Flee",
			() =>
			{
				steeringContext.maxSpeed = blackboard.fleeSpeed;
				flee.target = blackboard.leviathan;
				flee.enabled = true;
			},
			() => { },
			() =>
			{
				steeringContext.maxSpeed = blackboard.speed;
				flee.enabled = false;
			}
		);

		State returnToGroup = new State("Return",
			() =>
			{
				arrive.target = blackboard.fishAttractor;
				arrive.enabled = true;
			},
			() => { },
			() => { arrive.enabled = false; }
		);

		Transition monsterIsNear = new Transition("Monster Is Near",
			() =>
			{
				return SensingUtils.DistanceToTarget(gameObject, blackboard.leviathan) <
				       blackboard.dangerousDistanceFromMonster;
			}
		);

		Transition monsterIsFar = new Transition("Monster Is Far",
			() =>
			{
				return SensingUtils.DistanceToTarget(gameObject, blackboard.leviathan) >
				       blackboard.safeDistanceFromMonster;
			}
		);

		Transition arrivedToGroup = new Transition("Arrived To Group",
			() =>
			{
				return SensingUtils.DistanceToTarget(gameObject, blackboard.fishAttractor) <
				       blackboard.groupArrivedDistance;
			}
		);
		
		AddStates(wanderInGroup, fleeFromMonster, returnToGroup);
		
		AddTransition(wanderInGroup, monsterIsNear, fleeFromMonster);
		AddTransition(returnToGroup, monsterIsNear, fleeFromMonster);
		AddTransition(fleeFromMonster, monsterIsFar, returnToGroup);
		AddTransition(returnToGroup, arrivedToGroup, wanderInGroup);
		
		initialState = wanderInGroup;
	}
}