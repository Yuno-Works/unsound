using System;
using UnityEngine;

namespace SilverDogGames.AI.Goap.States
{
    using ReGoap.Unity.FSM;
    using ReGoapFSM = ReGoap.Unity.FSM;
    using SilverDogGames.Networking.FSM;
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

            // Impair player
            if (player.TryGetComponent(out PlayerStateMachine psm))
            {
                psm.RPC_Impaired();
            }
            currentState = ActionState.Success;
            Exit();
        }
        #endregion

        #region StateHandler
        public override void Init(ReGoapFSM.StateMachine stateMachine)
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
    }
}
