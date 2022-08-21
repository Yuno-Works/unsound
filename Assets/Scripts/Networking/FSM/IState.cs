namespace SilverDogGames.Networking.FSM
{
    internal interface IState
    {
        public string Name { get; }
        public bool IsActive { get; }
        public StateMachine StateMachine { get; }

        public void Init(string name, StateMachine stateMachine);
        public void Enter();
        public void UpdateLogic();
        public void UpdatePhysics();
        public void Exit();
    }
}
