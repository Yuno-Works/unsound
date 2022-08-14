namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class IdleState : BaseState
    {
        private PlayerStateMachine _psm;

        public IdleState(PlayerStateMachine stateMachine) : base("Idle", stateMachine)
        {
            _psm = stateMachine;
        }
    }
}
