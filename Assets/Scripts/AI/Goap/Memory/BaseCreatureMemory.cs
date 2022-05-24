namespace SilverDogGames.AI.Goap.Memory
{
    using ReGoap.Unity;

    public class BaseCreatureMemory : ReGoapMemory<string, object>
    {
        public void Init()
        {
            Awake();
        }

        public void SetValue(string key, object value)
        {
            state.Set(key, value);
        }
    }
}
