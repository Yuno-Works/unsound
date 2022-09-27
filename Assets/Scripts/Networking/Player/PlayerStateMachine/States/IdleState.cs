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

        public override void Init(StateMachine stateMachine)
        {
            Name = "Idle";
            base.Init(stateMachine);
        }
        public override void Enter()
        {
            base.Enter();
        }
        public override void UpdateLogic()
        {
            if (!isLocalPlayer) return;
            base.UpdateLogic();
            movementInput = playerController.GetMovementValue();
            if (movementInput.sqrMagnitude > Mathf.Epsilon)
            {
                StateMachine.ChangeState<MovingState>();
            }
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
