namespace SilverDogGames.Networking.FSM
{
    using HFPS.Player;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class IdleState : BaseState
    {
        [SerializeField] private PlayerController playerController = null;
        [SerializeField, ReadOnly] private Vector2 movementInput;

        public void Init(PlayerStateMachine stateMachine)
        {
            base.Init("Idle", stateMachine);
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            movementInput = playerController.GetMovementValue();
            if (movementInput.sqrMagnitude > Mathf.Epsilon)
            {
                stateMachine.ChangeState<MovingState>();
            }
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
