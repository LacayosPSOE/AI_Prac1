using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Leviathan_HuntingPlayer", menuName = "Finite State Machines/FSM_Leviathan_HuntingPlayer", order = 1)]
public class FSM_Leviathan_HuntingPlayer : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
         private Arrive arrive;
    private LEVIATHAN_Blackboard blackboard;
    private SteeringContext steeringContext;
    private float stamina;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
         arrive = GetComponent<Arrive>();
        blackboard = GetComponent<LEVIATHAN_Blackboard>();
        steeringContext = GetComponent<SteeringContext>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.OnExit();
    }

    public override void OnConstruction()
    {        
        FiniteStateMachine EAT = ScriptableObject.CreateInstance<FSM_Leviathan_Eat>();
        EAT.name = "EAT";
         State chasePlayer = new State("Chase_Player",
            () => { arrive.target = blackboard.Player; arrive.enabled = true; stamina = 0f; },
            () => { stamina += Time.deltaTime;},
            () => { arrive.enabled = false; }
        );

        State eatPlayer = new State("Eat_State",
            () => { blackboard.eatMaxTimer = 0f; },
            () => { blackboard.eatMaxTimer += Time.deltaTime;},
            () => {Destroy(blackboard.Player); }
        );
         Transition playerTooClose = new Transition("Player_Too_Close",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) < blackboard.playerChaseRadius; },
            () => { }
        );
               Transition playerWithinEatRange = new Transition("Player_Within_Eat_Range",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) < blackboard.eatRadius; },
            () => { }
        );
                       Transition playerEaten = new Transition("Player_Within_Eat_Range",
            () => { return blackboard.eatMaxTimer >= 1.5f; },
            () => { }
        );
        Transition playerFar = new Transition("Player_Far",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) >= blackboard.playerChaseRadius || blackboard.maxStamina <= stamina; },
            () => { }
        );

        AddStates(EAT, chasePlayer, eatPlayer);
        AddTransition(chasePlayer, playerWithinEatRange, eatPlayer);
        AddTransition(EAT, playerTooClose, chasePlayer);
        AddTransition(chasePlayer, playerFar, EAT);
        AddTransition(eatPlayer, playerEaten, EAT);


        initialState = EAT;


    }
}
