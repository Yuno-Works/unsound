using Mirror;

namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    internal class StateMachine : NetworkBehaviour
    {
        [SerializeField] private bool isDebug = false;
        [SerializeField, ReadOnly] private BaseState currentState;

        public void ChangeState(BaseState newState)
        {
            currentState.Exit();

            currentState = newState;
            currentState.Enter();
        }

        protected virtual BaseState GetInitialState()
        {
            return null;
        }

        private void Start()
        {
            currentState = GetInitialState();
        }

        private void Update()
        {
            if (currentState != null)
                currentState.UpdateLogic();
        }

        private void LateUpdate()
        {
            if (currentState != null)
                currentState.UpdatePhysics();
        }

        private void OnGUI()
        {
            if (!isDebug || !isLocalPlayer) return;
            string content = currentState != null ? currentState.Name : "(no current state)";
            GUILayout.Label($"<color='white'><size=40>{content}</size></color>");
        }
    }
}
