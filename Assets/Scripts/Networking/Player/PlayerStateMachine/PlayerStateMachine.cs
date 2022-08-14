namespace SilverDogGames.Networking.FSM
{
    using Sirenix.OdinInspector;

    internal class PlayerStateMachine : StateMachine
    {
        [ReadOnly] public IdleState idleState;
        [ReadOnly] public MovingState movingState;

        private void Awake()
        {
            idleState = new IdleState(this);
            movingState = new MovingState(this);
        }

        protected override BaseState GetInitialState()
        {
            return idleState;
        }
    }
}
