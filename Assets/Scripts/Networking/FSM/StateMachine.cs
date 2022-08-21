using Mirror;

namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    internal class StateMachine : NetworkBehaviour
    {
        public BaseState CurrentState { get => _currentState; private set => _currentState = value; }

        [SerializeField] private bool isDebug = false;
        [SerializeField, ReadOnly] private BaseState _currentState = null;
        [SerializeField, SyncVar(hook = nameof(OnNameChanged))] private string currentStateName = null;
        [Header("Debug")]
        [SerializeField] private UnityEngine.UI.Text stateNameText = null;

        public void ChangeState<T>() where T : BaseState
        {
            if (!isLocalPlayer) { return; }
            if (TryGetComponent(out T state))
            {
                if (state == CurrentState) return;
                CurrentState.Exit();
                CurrentState = state;
                CurrentState.Enter();

                if (!isServer)
                    CmdUpdateStateName(CurrentState.Name);
                else
                    currentStateName = CurrentState.Name;
            }
            else
            {
                Debug.LogErrorFormat("[{0}] does not contain state: [{1}]", GetType(), state);
            }
        }

        protected virtual BaseState GetInitialState()
        {
            return null;
        }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnGUIUpdate() { }

        #region Hooks
        private void OnNameChanged(string oldName, string newName)
        {
            stateNameText.text = newName;
        }
        #endregion
        #region RPCs
        [Command]
        private void CmdUpdateStateName(string name)
        {
            currentStateName = name;
        }
        #endregion
        private void Start()
        {
            if (!isLocalPlayer) { return; }
            CurrentState = GetInitialState();
        }
        private void Update()
        {
            OnUpdate();
            if (!isLocalPlayer) { return; }
            if (CurrentState != null)
                CurrentState.UpdateLogic();
        }
        private void LateUpdate()
        {
            if (!isLocalPlayer) { return; }
            if (CurrentState != null)
                CurrentState.UpdatePhysics();
        }
        private void OnGUI()
        {
            OnGUIUpdate();
            if (isDebug && isLocalPlayer)
            {
                string content = CurrentState != null ? CurrentState.Name : "(no current state)";
                GUILayout.Label($"<color='white'><size=40>{content}</size></color>");
            }
        }
    }
}
