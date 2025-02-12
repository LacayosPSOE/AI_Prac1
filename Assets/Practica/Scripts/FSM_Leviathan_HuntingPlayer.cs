using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Leviathan_HuntingPlayer", menuName = "Finite State Machines/FSM_Leviathan_HuntingPlayer", order = 1)]
public class FSM_Leviathan_HuntingPlayer : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
         private Seek seek;
    private LEVIATHAN_Blackboard blackboard;
    private SteeringContext steeringContext;
    private float stamina;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        seek = GetComponent<Seek>();
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
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        FiniteStateMachine EAT = ScriptableObject.CreateInstance<FSM_Leviathan_Eat>();
        EAT.name = "EAT";
         State chasePlayer = new State("Chase_Player",
            () => { seek.target = blackboard.Player; seek.enabled = true; stamina = 0f; },
            () => { stamina += Time.deltaTime;},
            () => { seek.enabled = false; }
        );

        State eatState = new State("Eat_State",
            () => { blackboard.eatMaxTimer = 0f; },
            () => { blackboard.eatMaxTimer += Time.deltaTime;},
            () => {blackboard.Player.SetActive(false); }
        );
  

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */ 
         Transition playerTooClose = new Transition("Player_Too_Close",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) < blackboard.playerChaseRadius; },
            () => { }
        );

        Transition playerFar = new Transition("Player_Far",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) >= blackboard.playerChaseRadius || blackboard.maxStamina <= stamina; },
            () => { }
        );

        AddStates(EAT, chasePlayer, eatState);
        AddTransition(EAT, playerTooClose, chasePlayer);
        AddTransition(chasePlayer, playerFar, EAT);

        initialState = EAT;



        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

    }
}
