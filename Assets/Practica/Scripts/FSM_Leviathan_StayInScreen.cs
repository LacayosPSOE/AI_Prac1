using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Leviathan_StayInScreen", menuName = "Finite State Machines/FSM_Leviathan_StayInScreen",
	order = 1)]
public class FSM_Leviathan_StayInScreen : FiniteStateMachine
{
	/* Declare here, as attributes, all the variables that need to be shared among
	 * states and transitions and/or set in OnEnter or used in OnExit
	 * For instance: steering behaviours, blackboard, ...*/

	private Arrive arrive;
	private WanderAround wanderAround;
	private LEVIATHAN_Blackboard blackboard;

	public override void OnEnter()
	{
		arrive = GetComponent<Arrive>();
		wanderAround = GetComponent<WanderAround>();
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
		FiniteStateMachine HUNT = ScriptableObject.CreateInstance<FSM_Leviathan_HuntingPlayer>();
		HUNT.name = "HUNT";

		State returnToScreen = new State("Return_To_Screen",
			() =>
			{
				arrive.target = wanderAround.attractor;
				arrive.enabled = true;
			},
			() => { },
			() => { arrive.enabled = false; }
		);

		Transition isOutsideBounds = new Transition("Monster_Outside_Screen",
			() =>
			{
				return SensingUtils.DistanceToTarget(gameObject, wanderAround.attractor) >
				       blackboard.screenBoundDistance;
			},
			() => { }
		);

		Transition isInsideBounds = new Transition("Monster_Inside_Bounds",
			() =>
			{
				return SensingUtils.DistanceToTarget(gameObject, wanderAround.attractor) <=
				       blackboard.screenSafeBounds;
			},
			() => { }
		);

		AddStates(HUNT, returnToScreen);
		AddTransition(HUNT, isOutsideBounds, returnToScreen);
		AddTransition(returnToScreen, isInsideBounds, HUNT);

		initialState = HUNT;
	}
}