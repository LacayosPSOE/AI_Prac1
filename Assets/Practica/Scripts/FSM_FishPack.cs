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
	private SteeringContext steeringContext;
	private FlockingAroundPlusAvoidance flocking;
	private Flee flee;

	public override void OnEnter()
	{
		blackboard = GetComponent<FISH_blackboard>();
		blackboard.leviathan = GameObject.FindWithTag("PREDATOR");
		steeringContext = GetComponent<SteeringContext>();
		flee = GetComponent<Flee>();
		flocking = GetComponent<FlockingAroundPlusAvoidance>();
		steeringContext.maxSpeed = blackboard.speed;
		base.OnEnter();
	}

	public override void OnExit()
	{
		base.DisableAllSteerings();
		base.OnExit();
	}

	public override void OnConstruction()
	{
		State wanderInGroup = new State("Wander",
			() =>
			{
				flocking.enabled = true;
			},
			() => { },
			() => { flocking.enabled = false; }
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
		
		AddStates(wanderInGroup, fleeFromMonster);
		
		AddTransition(wanderInGroup, monsterIsNear, fleeFromMonster);
		AddTransition(fleeFromMonster, monsterIsFar, wanderInGroup);
		
		initialState = wanderInGroup;
	}
}