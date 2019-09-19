

//A basic state template for all types of state machine used in project 

using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AbstractClasses
{
    public abstract class State
    {
        protected readonly Controller Controller;

        public bool CurrentlyActive { get; private set; } = false;

        public void SetCurrentlyActive(bool value)
        {
//            Debug.Log("called from abstract class: " + this.GetType() + " || value: " + value);
            CurrentlyActive = value;
        }
        
        protected State(Controller cont)
        {
            Controller = cont;
            
            Controller.AllStates.Add(this.GetType(), this);
        }

        public virtual void Start(){}

        public virtual void OnStateEnter()
        {
//            CurrentlyActive = true;
        }
        
        public abstract void Update();

        public virtual void OnStateExit()
        {
//            CurrentlyActive = false;
        }
    }
}
