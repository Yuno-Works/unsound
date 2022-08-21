namespace SilverDogGames.AI.Goap.Goals
{
    using ReGoap.Unity;

    public class AttackPlayerGoal : ReGoapGoal<string, object>
    {
        protected override void Awake()
        {
            base.Awake();
            goal.Set("attackedPlayer", true);
        }

        public override string ToString()
        {
            return string.Format("GoapGoal('{0}')", Name);
        }
    }
}
