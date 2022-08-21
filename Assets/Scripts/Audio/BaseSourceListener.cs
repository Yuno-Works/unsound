using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.Audio
{
    using Sirenix.OdinInspector;

    public class BaseSourceListener : SerializedMonoBehaviour
    {
        [SerializeField] protected float DetectionRadius = 50f;
        [SerializeField] protected LayerMask SourceMask;
        [SerializeField] protected float SampleUpdateStep = 0.1f;
        [SerializeField, ReadOnly] protected List<AudioSourceData> SourceData = new List<AudioSourceData>();
        [SerializeField, ReadOnly] private float updateTime = 0f;

        public AudioSourceData[] GetSourceData() => SourceData.ToArray();
        protected virtual void GetSources() { }
        protected virtual void ProcessSourceData() { }

        // Start is called before the first frame update
        private void Start()
        {
            SourceData = new List<AudioSourceData>();
        }
        private void FixedUpdate()
        {
            GetSources();
        }
        private void Update()
        {
            updateTime += Time.deltaTime;
            if (updateTime >= SampleUpdateStep)
            {
                updateTime = 0f;
                ProcessSourceData();
            }
        }
    }
}
