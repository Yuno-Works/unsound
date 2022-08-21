namespace SilverDogGames.Networking.FSM
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    internal class PlayerStateMachine : StateMachine
    {
        public BaseState InitialState => initialState;
        [SerializeField] private BaseState initialState = null;

        protected override BaseState GetInitialState()
        {
            return InitialState;
        }
        protected override void OnUpdate()
        {
            if (!isLocalPlayer) return;
            if (Keyboard.current[Key.I].wasPressedThisFrame)
            {
                if (GetState<ImpairedState>().IsActive)
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
