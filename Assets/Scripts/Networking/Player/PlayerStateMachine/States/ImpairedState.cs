namespace SilverDogGames.Networking.FSM
{
    using HFPS.Player;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ImpairedState : BaseState
    {
        [SerializeField] private PlayerController playerController = null;

        public override void Init(StateMachine stateMachine)
        {
            Name = "Impaired";
            base.Init(stateMachine);
        }
        public override void Enter()
        {
            base.Enter();
            playerController.isControllable = false;
            playerController.CharacterControl.enabled = false;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
        }
        public override void Exit()
        {
            base.Exit();
            playerController.isControllable = true;
            playerController.CharacterControl.enabled = true;
        }
    }
}
