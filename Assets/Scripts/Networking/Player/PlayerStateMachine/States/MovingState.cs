namespace SilverDogGames.Networking.FSM
{
    using HFPS.Player;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class MovingState : BaseState
    {
        [SerializeField] private PlayerController playerController = null;
        [SerializeField, ReadOnly] private Vector2 movementInput;

        public void Init(PlayerStateMachine stateMachine)
        {
            Init("Moving", stateMachine);
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            movementInput = playerController.GetMovementValue();
            if (movementInput.sqrMagnitude <= Mathf.Epsilon)
            {
                StateMachine.ChangeState<IdleState>();
            }
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
