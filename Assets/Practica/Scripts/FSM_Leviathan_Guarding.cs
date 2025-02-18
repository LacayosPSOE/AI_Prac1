using FSMs;
using UnityEngine;
using Steerings;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "FSM_Leviathan_Guarding", menuName = "Finite State Machines/FSM_Leviathan_Guarding",
	order = 1)]
public class FSM_Leviathan_Guarding : FiniteStateMachine
{
	private Arrive arrive;
	private LEVIATHAN_Blackboard blackboard;
	private float timer;

	public override void OnEnter()
	{
		arrive = GetComponent<Arrive>();
		blackboard = GetComponent<LEVIATHAN_Blackboard>();

		base.OnEnter(); // do not remove
	}

	public override void OnExit()
	{
		base.DisableAllSteerings();
		base.OnExit();
	}

	public override void OnConstruction()
	{
		FiniteStateMachine EAT = ScriptableObject.CreateInstance<FSM_Leviathan_Eat>();
		EAT.name = "EAT";

		State goHome = new State("Go_Home",
			() =>
			{
				arrive.target = blackboard.Home;
				blackboard.speed = blackboard.speed * 2;
				arrive.enabled = true;
			},
			() => { },
			() =>
			{
				arrive.enabled = false;
				blackboard.speed = blackboard.speed / 2;
			});

		State guard = new State("Guard",
			() => { timer = 0f; },
			() => { timer += Time.deltaTime; },
			() => { }
		);
		
		Transition playerTooCloseHome = new Transition("Player_Too_Close_Home",
			() => { return SensingUtils.DistanceToTarget(blackboard.Player, blackboard.Home) < blackboard.homeArrivedRadius; },
			() => { }
		);

		Transition arrivedHome = new Transition("Arrived_Home",
			() => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Home) < blackboard.homeArrivedRadius; },
			() => { }
		);

		Transition stopGuarding = new Transition("Stop_Guarding",
			() => { return timer >= blackboard.maxGuardTimer; },
			() => { }
		);

		AddStates(EAT, goHome, guard);
		AddTransition(EAT, playerTooCloseHome, goHome);
		AddTransition(goHome, arrivedHome, guard);
		AddTransition(guard, stopGuarding, EAT);
		
		initialState = EAT;

		//Falta que el leviathan vaya a por el player cuando el player esta en su guarida
	}
}