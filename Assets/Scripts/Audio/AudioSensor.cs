namespace SilverDogGames.AI.Goap.Sensors
{
    using System.Linq;
    using ReGoap.Core;
    using SilverDogGames.Audio;
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioSensor : AudioSensorBase<string, object>
    {
        [SerializeField] private List<AudioSourceData> sourceDataCache = new List<AudioSourceData>();

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnBeat(AudioSourceData? sourceData)
        {
            base.OnBeat(sourceData);

            if (sourceData.HasValue)
            {
                sourceDataCache.Add(sourceData.Value);
            }
        }
        public override void Init(IReGoapMemory<string, object> memory)
        {
            base.Init(memory);
        }

        public override void UpdateSensor()
        {
            var worldState = memory.GetWorldState();
            worldState.Set("visibleTargets", sourceDataCache.Select(s => s.Position).ToArray());
            sourceDataCache?.Clear();
        }
    }
}
