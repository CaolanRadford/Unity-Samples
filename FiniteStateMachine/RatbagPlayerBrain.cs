using System.Collections.Generic;
using AbstractClasses;
using Components;
using StateClasses.Ratbag;
using StateClasses.Ratbag.AnimationStates;
using StateClasses.Ratbag.GameplayStates;
using UnityEngine;
using Floored = StateClasses.Ratbag.AnimationStates.Floored;
using GettingUp = StateClasses.Ratbag.GameplayStates.GettingUp;
using Tripping = StateClasses.Ratbag.GameplayStates.Tripping;

namespace Controllers
{
//Controller for the main player character in my game Ratbag, this is a state state machine that utilises non-monobehaviour state classes for handling both animation and gameplay
    public class RatbagPlayerBrain : Controller, IPlayerInput2D
    {
        //state machines
        public Dictionary <object, RatbagPlayerState> AvailableStates { get; } = new Dictionary<object, RatbagPlayerState>();

        public Dictionary<object, RatbagAnimationSubState> AvailableAnimationStates { get; } = new Dictionary<object, RatbagAnimationSubState>();

        public RatbagPlayerState CurrentState {get; private set;}
        public RatbagAnimationSubState CurrentAnimationState {get; private set;}
        
        void OnEnable()
        {
            InstantiateStates();
            InitialiseAnimationStates();
        }

        void Start()
        {
            SetPlayerToInitialSpawningState();
        }

        private void Update()
        {
            CurrentState.Update();
            CurrentAnimationState.Update();
        }    
        
        private void LateUpdate()
        {
            CurrentState.LateUpdate();
        }
        private void FixedUpdate()
        {
            CurrentState.PhysicsUpdate();
        }

        public void SetState(RatbagPlayerState state) 
        {
            if(CurrentState != null)
            {
                CurrentState.OnStateExit();
                CurrentState.SetCurrentlyActive(false);
            }

            CurrentState = state;
//        print("Ratbag in player state: " + currentState);

            if(CurrentState != null)
            {
                CurrentState.OnStateEnter();
                CurrentState.SetCurrentlyActive(true);
            }
        }    
        
        public void SetAnimationState(RatbagAnimationSubState state)
        {
            if(CurrentAnimationState != null)
            {
                CurrentAnimationState.OnStateExit();
                CurrentAnimationState.SetCurrentlyActive(false);
            }

            CurrentAnimationState = state;
            
    //            Debug.Log("Ratbag animation in state: " + currentAnimationState);
            
            if(CurrentAnimationState != null)
            {
                CurrentAnimationState.OnStateEnter();
                CurrentAnimationState.SetCurrentlyActive(true);
            }
        }

        //Hand off Player input to active states
        public void OnHorizontalAxis(float horizontalAxis) => CurrentState.OnHorizontalAxis(horizontalAxis);
        public void OnJumpButton() => CurrentState.OnJumpButton();
        public void OnNoButton() => CurrentState.OnNoButton();

        //setup
        private void InstantiateStates()
        {
            AvailableStates.Add(typeof(UpAndActive), new UpAndActive(this));
            AvailableStates.Add(typeof(AirBorne), new AirBorne(this));
            AvailableStates.Add(typeof(Tripping), new Tripping(this, GetComponent<Collider2D>()));
            AvailableStates.Add(typeof(StateClasses.Ratbag.GameplayStates.Floored), new StateClasses.Ratbag.GameplayStates.Floored(this));
            AvailableStates.Add(typeof(GettingUp), new GettingUp(this));
            AvailableStates.Add(typeof(Spawning), new Spawning(this));
        }
        
        private void InitialiseAnimationStates()
        {
            AvailableAnimationStates.Add(typeof(Falling), new Falling(this));
            AvailableAnimationStates.Add(typeof(Floored), new Floored(this));
            AvailableAnimationStates.Add(typeof(Idle), new Idle(this));
            AvailableAnimationStates.Add(typeof(Jumping), new Jumping(this));
            AvailableAnimationStates.Add(typeof(Running), new Running(this));
            AvailableAnimationStates.Add(typeof(Turning), new Turning(this));
            AvailableAnimationStates.Add(typeof(StateClasses.Ratbag.AnimationStates.Tripping), new StateClasses.Ratbag.AnimationStates.Tripping(this));
            AvailableAnimationStates.Add(typeof(StateClasses.Ratbag.AnimationStates.GettingUp), new StateClasses.Ratbag.AnimationStates.GettingUp(this));
        }
        private void SetPlayerToInitialSpawningState()
        {
            SetState(AvailableStates[typeof(Spawning)]);
        }
    }
}