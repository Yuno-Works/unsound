namespace SilverDogGames.Networking.FSM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class MovingState : BaseState
    {
        private PlayerStateMachine _psm;

        public MovingState(PlayerStateMachine stateMachine) : base("Moving", stateMachine)
        {
            _psm = stateMachine;
        }
    }
}
