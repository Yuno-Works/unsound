namespace SilverDogGames.AI.Goap.Goals
{
    using ReGoap.Unity;

    public class PatrolGoal : ReGoapGoal<string, object>
    {
        protected override void Awake()
        {
            base.Awake();
            goal.Set("patrol", true);
        }

        public override string ToString()
        {
            return string.Format("GoapGoal('{0}')", Name);
        }
    }
}
