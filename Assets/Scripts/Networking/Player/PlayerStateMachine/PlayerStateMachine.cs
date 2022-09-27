namespace SilverDogGames.Networking.FSM
{
    using global::Mirror;
    using UnityEngine;
    using UnityEngine.InputSystem;

    internal class PlayerStateMachine : StateMachine
    {
        public BaseState InitialState => initialState;
        [SerializeField] private BaseState initialState = null;

        #region State overrides
        [ClientRpc] public void RPC_Idle() => ChangeState<ImpairedState>();
        [ClientRpc] public void RPC_Moving() => ChangeState<MovingState>();
        [ClientRpc] public void RPC_Impaired() => ChangeState<ImpairedState>();
        #endregion

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
