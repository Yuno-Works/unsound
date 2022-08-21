namespace SilverDogGames.AI.Goap.Sensors
{
    using ReGoap.Core;
    using ReGoap.Unity;

    public class NoiseSensor : ReGoapSensor<string, object>
    {
        public override void Init(IReGoapMemory<string, object> memory)
        {
            base.Init(memory);
        }

        public override void UpdateSensor()
        {
            throw new System.NotImplementedException();
        }
    }
}
