namespace SilverDogGames.Networking.FSM
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    internal class PlayerStateMachine : StateMachine
    {
        public BaseState InitialState => initialState;
        [SerializeField] private BaseState initialState = null;

        public void ServerChangeState<T>() where T : BaseState
        {
            ChangeState<T>();
        }

        protected override BaseState GetInitialState()
        {
            return InitialState;
        }
#if UNITY_EDITOR
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
#endif
    }
}
