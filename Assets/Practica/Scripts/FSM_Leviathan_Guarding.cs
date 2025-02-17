using FSMs;
using UnityEngine;
using Steerings;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "FSM_Leviathan_Guarding", menuName = "Finite State Machines/FSM_Leviathan_Guarding", order = 1)]
public class FSM_Leviathan_Guarding : FiniteStateMachine
{

    private Arrive arrive;
    private LEVIATHAN_Blackboard blackboard;
    private SteeringContext steeringContext;
    private float timer;
    public override void OnEnter()
    {

        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<LEVIATHAN_Blackboard>();
        steeringContext = GetComponent<SteeringContext>();
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {

        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine EAT = ScriptableObject.CreateInstance<FSM_Leviathan_Eat>();
        EAT.name = "EAT";
        FiniteStateMachine HUNT = ScriptableObject.CreateInstance<FSM_Leviathan_HuntingPlayer>();
        HUNT.name = "HUNT";

        State goHome = new State("Go_Home",
            () =>
            {
                arrive.target = blackboard.Home;
                blackboard.speed = blackboard.speed * 2;
                arrive.enabled = true;
            },
            () => { },
            () =>{arrive.enabled = false; blackboard.speed = blackboard.speed / 2;});
        State Guard = new State("Guard",
        () =>{},
        () => { timer += Time.deltaTime; },
        () => { timer = 0;});


        Transition arrivedHome = new Transition("Arrived_Home",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Home) < blackboard.eatRadius; }, 
            () => { }  
        );
        Transition stopGuarding = new Transition("Stop_Guarding",
    () => { return timer >= blackboard.maxGuardTimer; },
    () => { }
);
        Transition hunt = new Transition("Hunt",
        () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.Player) < blackboard.playerChaseRadius; },
        () => { }
         );

        AddStates(EAT, HUNT, goHome, Guard);
        AddTransition(goHome,arrivedHome,Guard);
        AddTransition(Guard,stopGuarding, EAT);
        AddTransition(EAT, hunt, HUNT);
        AddTransition(goHome, hunt, HUNT);
        AddTransition(Guard, hunt, HUNT);
        //Falta que el leviathan vaya a por el player cuando el player esta en su guarida
    }
}
