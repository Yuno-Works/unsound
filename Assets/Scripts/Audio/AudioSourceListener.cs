using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.Audio
{
    using System.Linq;

    public class AudioSourceListener : BaseSourceListener
    {
        [SerializeField] private List<AudioSource> allAudioSources = new List<AudioSource>();
        [SerializeField] private int sampleLength = 1024;
        [SerializeField] private float offset = 0f;
        [SerializeField] private float sensitivity = 10f;
        private float[] audioSpectrum = null;

        // Start is called before the first frame update
        private void Start()
        {
            allAudioSources = new List<AudioSource>();

            // Initialize buffer
            audioSpectrum = new float[sampleLength];
        }

        protected override void GetSources()
        {
            allAudioSources.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, DetectionRadius, SourceMask);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out AudioSource audioSource))
                {
                    allAudioSources.Add(audioSource);
                }
            }
        }
        protected override void ProcessSourceData()
        {
            SourceData.Clear();
            if (allAudioSources.Count > 0 && sampleLength > 0)
            {
                foreach (AudioSource source in allAudioSources)
                {
                    if (source.clip?.loadState == AudioDataLoadState.Loaded)
                    {
                        source.clip.GetData(audioSpectrum, source.timeSamples);
                        float loudness = 0f;
                        float distance = Mathf.Max(1f, Vector3.Distance(transform.position, source.transform.position));
                        // Calculate clip loudness
                        foreach (var sample in audioSpectrum)
                        {
                            loudness += Mathf.Abs(sample) + offset;
                        }
                        loudness /= sampleLength;
                        loudness *= (-Mathf.Pow(distance - 1, 2) / (sensitivity * sensitivity)) + 1;
                        loudness *= source.volume;
                        loudness = Mathf.Clamp01(loudness * sensitivity);
                        SourceData.Add(new AudioSourceData(source.name, loudness, source.transform.position));

#if UNITY_EDITOR
                        Vector3 dir = (source.transform.position - transform.position);
                        Debug.DrawRay(transform.position, dir * loudness, Color.white, SampleUpdateStep, true);
#endif
                    }
                }
                SourceData.OrderByDescending(sourceData => sourceData.Loudness);
            }
        }
    }
}
