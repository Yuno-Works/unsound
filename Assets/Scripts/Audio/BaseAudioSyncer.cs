using UnityEngine;

namespace SilverDogGames.Audio
{
    using Sirenix.OdinInspector;
    using System.Linq;

    public class BaseAudioSyncer : SerializedMonoBehaviour
    {
        [SerializeField] private float threshold = 0.25f;
        [SerializeField] private float timeStep = 0.1f;
        [SerializeField] protected float timeToBeat = 0.05f;
        [SerializeField] protected float restSmoothTime = 2f;

        [SerializeField] private BaseSourceListener[] sourceListeners = null;
        [SerializeField, ReadOnly] private float loudness = 0f;
        [SerializeField, ReadOnly] private float prevLoudness = 0f;
        [SerializeField, ReadOnly] private float timer = 0f;

        protected bool m_isBeat;

        public virtual void OnBeat(AudioSourceData? sourceData)
        {
            timer = 0;
            m_isBeat = true;
        }
        public virtual void OnUpdate()
        {
            foreach (var listener in sourceListeners)
            {
                ProcessSource(listener.GetSourceData());
            }
            timer += Time.deltaTime;
        }

        private void ProcessSource(params AudioSourceData[] sources)
        {
            // Update audio value "loudness"
            foreach (var sourceData in sources)
            {
                loudness = sourceData.Loudness;

                if (loudness > threshold)
                {
                    // If minimum beat interval is reached
                    if (timer > timeStep)
                    {
                        OnBeat(sourceData);
                    }
                }

                //// If loudness value went below the threshold during this frame
                //if (prevLoudness > threshold &&
                //    loudness <= threshold)
                //{
                //    // If minimum beat interval is reached
                //    if (timer > timeStep)
                //    {
                //        OnBeat(sourceData);
                //        prevLoudness = loudness;
                //    }
                //}

                //// If loudness value went above the threshold during this frame
                //if (prevLoudness <= threshold &&
                //    loudness > threshold)
                //{
                //    // If minimum beat interval is reached
                //    if (timer > timeStep)
                //    {
                //        OnBeat(sourceData);
                //        prevLoudness = loudness;
                //    }
                //}
            }
        }

        private void Update()
        {
            OnUpdate();
        }
    }
}
