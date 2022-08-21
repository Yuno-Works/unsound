using Mirror;

namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    internal class BaseState : NetworkBehaviour, IState
    {
        public string Name { get => _name; private set => _name = value; }
        public bool IsActive { get => isActive; private set => isActive = value; }
        public StateMachine StateMachine { get => stateMachine; private set => stateMachine = value; }
        private string _name;
        private bool isActive = false;
        private StateMachine stateMachine = null;

        public virtual void Init(string name, StateMachine stateMachine)
        {
            Name = string.Format("{0} ({1})", name, stateMachine.isLocalPlayer ? "local" : "remote");
            StateMachine = stateMachine;
        }
        public virtual void Enter() { IsActive = true; }
        public virtual void UpdateLogic() { }
        public virtual void UpdatePhysics() { }
        public virtual void Exit() { IsActive = false; }

        private void Start()
        {
            enabled = isLocalPlayer;
        }
    }
}
