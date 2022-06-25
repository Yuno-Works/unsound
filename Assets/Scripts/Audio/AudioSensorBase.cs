using System.Collections;
using UnityEngine;

namespace SilverDogGames.AI.Goap.Sensors
{
    using SilverDogGames.Audio;
    using ReGoap.Core;

    public class AudioSensorBase<T, W> : BaseAudioSyncer, IReGoapSensor<T, W>
    {
        protected IReGoapMemory<T, W> memory;
        public virtual void Init(IReGoapMemory<T, W> memory)
        {
            this.memory = memory;
        }

        public virtual IReGoapMemory<T, W> GetMemory()
        {
            return memory;
        }

        public virtual void UpdateSensor()
        {
            Debug.LogFormat("{0} UpdateSensor()", this);
        }
    }
}
