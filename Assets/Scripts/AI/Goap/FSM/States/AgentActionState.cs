using System;
using UnityEngine;
using Mirror;

namespace SilverDogGames.AI.Goap.States
{
    using ReGoap.Unity.FSM;
    using ReGoap.Utilities;
    using Sirenix.OdinInspector;

    public class AgentActionState : NetworkedSmState
    {
        #region Models

        public enum ActionState
        {
            Disabled, Active, Success, Failure
        }
        public enum ActionType
        {
            None, Attack
        }

        #endregion

        public ActionState CurrentState => currentState;

        [SerializeField] private Action onDoneCallback = null;
        [SerializeField] private Action onFailureCallback = null;
        [Space]
        [SerializeField, ReadOnly] private ActionState currentState = ActionState.Disabled;
        [SerializeField, ReadOnly] private ActionType currentAction = ActionType.None;

        #region Action Handlers
        public void Attack(Transform player, Action doneCallback, Action failureCallback)
        {
            currentAction = ActionType.Attack;
            onDoneCallback = doneCallback;
            onFailureCallback = failureCallback;

            // Debug
            RpcMovePlayer(player, new Vector3(0, 3, -3));
            currentState = ActionState.Success;
            Exit();
        }
        #endregion

        #region StateHandler
        public override void Init(StateMachine stateMachine)
        {
            base.Init(stateMachine);
            var transition = new SmTransition(GetPriority(), Transition);
            var doneTransition = new SmTransition(GetPriority(), DoneTransition);
            stateMachine.GetComponent<AgentIdleState>().Transitions.Add(transition);
            Transitions.Add(doneTransition);
        }

        private Type DoneTransition(ISmState state)
        {
            if (currentState != ActionState.Active)
                return typeof(AgentIdleState);
            return null;
        }

        private Type Transition(ISmState state)
        {
            if (currentState == ActionState.Active)
                return GetType();
            return null;
        }

        public override void Enter()
        {
            currentState = ActionState.Active;
        }

        public override void Exit()
        {
            currentAction = ActionType.None;
            if (currentState == ActionState.Success)
                onDoneCallback?.Invoke();
            else
                onFailureCallback?.Invoke();
            currentState = ActionState.Disabled;
        }
        #endregion

        #region Net Actions
        [ClientRpc] // (includeOwner = true)
        private void RpcMovePlayer(Transform player, Vector3 position)
        {
            player.transform.position = position;
        }
        #endregion
    }
}
