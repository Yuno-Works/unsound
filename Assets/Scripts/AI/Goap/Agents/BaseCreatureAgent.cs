using Sirenix.OdinInspector;

namespace SilverDogGames.AI.Goap.Agents
{
    using ReGoap.Unity;

    public class BaseCreatureAgent : ReGoapAgent<string, object>
    {
        [Button]
        protected void Init()
        {
            Awake();
        }
    }
}
