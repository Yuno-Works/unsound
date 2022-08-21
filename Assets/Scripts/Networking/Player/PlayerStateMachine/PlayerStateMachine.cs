namespace SilverDogGames.Networking.FSM
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    internal class PlayerStateMachine : StateMachine
    {
        public IdleState IdleState => idleState;
        public MovingState MovingState => movingState;

        private IdleState idleState = null;
        private MovingState movingState = null;
        private ImpairedState impairedState = null;

        private void Awake()
        {
            idleState = GetComponent<IdleState>();
            movingState = GetComponent<MovingState>();
            impairedState = GetComponent<ImpairedState>();
            idleState.Init(this);
            movingState.Init(this);
            impairedState.Init(this);
        }

        protected override BaseState GetInitialState()
        {
            return idleState;
        }
        protected override void OnUpdate()
        {
            if (!isLocalPlayer) return;
            if (Keyboard.current[Key.I].wasPressedThisFrame)
            {
                if (impairedState.IsActive)
                {
                    ChangeState<IdleState>();
                }
                else
                {
                    ChangeState<ImpairedState>();
                }
            }
        }
    }
}
