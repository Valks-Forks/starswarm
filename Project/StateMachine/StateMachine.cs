﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSwarm.Project.StateMachine
{
    public class StateMachine : Node
    {
        [Export]
        public NodePath InitialState { get; set; } = new NodePath();

        public State State {get { return State; } set {
                State = value;
                _stateName = State.Name;
            }
        }

        protected string? _stateName;

        public StateMachine()
        {
            AddToGroup("state_machine");
        }

        public override async void _Ready()
        {
            State = (State)GetNode(InitialState);
            _stateName = State.Name;
            await ToSignal(Owner, "ready");
            State.Enter();
        }

        public override void _UnhandledInput(InputEvent inputEvent)
        {
           State.UnhandledInput(inputEvent);
        }

        public override void _PhysicsProcess(float delta)
        {
            State.PhysicsProcess(delta);
        }

        public void TransitionTo(string targetStatePath, Dictionary<string, string>? msg = null)
        {
            if(msg == null)
                msg = new Dictionary<string, string>();

            if(!HasNode(targetStatePath))
                return;

            var targetState = GetNode<State>(targetStatePath);

            State.Exit();
            this.State = targetState;
            State.Enter(msg);
        }
    }
}