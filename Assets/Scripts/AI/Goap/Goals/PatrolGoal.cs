namespace SilverDogGames.AI.Goap.Goals
{
    using ReGoap.Core;
    using ReGoap.Unity;

    public class PatrolGoal : ReGoapGoal<string, object>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override ReGoapState<string, object> GetGoalState()
        {
            goal.Set("patrol", true);
            return base.GetGoalState();
        }

        public override string ToString()
        {
            return string.Format("GoapGoal('{0}')", Name);
        }
    }
}
