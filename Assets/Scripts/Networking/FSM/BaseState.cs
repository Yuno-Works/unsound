using Mirror;

namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    internal class BaseState : NetworkBehaviour, IState
    {
        public string Name { get => _name; protected set => _name = value; }
        public bool IsActive { get => isActive; private set => isActive = value; }
        protected StateMachine stateMachine;
        protected string _name;
        [SerializeField, ReadOnly] private bool isActive = false;

        public virtual void Init(string name, StateMachine stateMachine)
        {
            Name = string.Format("{0} ({1})", name, stateMachine.isLocalPlayer ? "local" : "remote");
            this.stateMachine = stateMachine;
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
