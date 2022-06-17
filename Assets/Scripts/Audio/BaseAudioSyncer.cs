using UnityEngine;

namespace SilverDogGames.Audio
{
    using Sirenix.OdinInspector;
    using System.Linq;

    public class BaseAudioSyncer : SerializedMonoBehaviour
    {
        [SerializeField] private float bias = 0.1f;
        [SerializeField] private float timeStep = 0.1f;
        [SerializeField] protected float timeToBeat = 0.05f;
        [SerializeField] protected float restSmoothTime = 2f;

        [SerializeField, Tooltip("AudioSourceManager instance required.")] private AudioSourceListener audioManager = null;
        [SerializeField, ReadOnly] private float loudness = 0f;
        [SerializeField, ReadOnly] private float prevLoudness = 0f;
        [SerializeField, ReadOnly] private float timer = 0f;

        protected bool m_isBeat;

        public virtual void OnBeat(Vector3? sourcePosition)
        {
            timer = 0;
            m_isBeat = true;
        }
        public virtual void OnUpdate()
        {
            // Update audio value "loudness"
            prevLoudness = loudness;
            var sourceData = audioManager.GetSourceData().FirstOrDefault();
            loudness = sourceData.Loudness;

            // If loudness value went below the bias during this frame
            if (prevLoudness > bias &&
                loudness <= bias)
            {
                // If minimum beat interval is reached
                if (timer > timeStep)
                    OnBeat(sourceData.Position);
            }

            // If loudness value went above the bias during this frame
            if (prevLoudness <= bias &&
                loudness > bias)
            {
                // If minimum beat interval is reached
                if (timer > timeStep)
                    OnBeat(sourceData.Position);
            }

            timer += Time.deltaTime;
        }

        private void Update()
        {
            if (audioManager != null)
            {
                OnUpdate();
            }
        }
    }
}
