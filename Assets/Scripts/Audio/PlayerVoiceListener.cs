using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.Audio
{
    using Sirenix.OdinInspector;
    using System.Linq;
    using Dissonance;

    public class PlayerVoiceListener : BaseSourceListener
    {
        [SerializeField] private float sensitivity = 10f;
        [SerializeField, ReadOnly] private List<IDissonancePlayer> allDissonancePlayers = new List<IDissonancePlayer>();
        [SerializeField, ReadOnly] private DissonanceComms comms = null;

        private void Start()
        {
            comms = FindObjectOfType<DissonanceComms>();
            allDissonancePlayers = new List<IDissonancePlayer>();
        }

        protected override void GetSources()
        {
            allDissonancePlayers.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, DetectionRadius, SourceMask);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IDissonancePlayer audioSource))
                {
                    allDissonancePlayers.Add(audioSource);
                }
            }
        }
        protected override void ProcessSourceData()
        {
            SourceData.Clear();
            if (allDissonancePlayers.Count > 0)
            {
                foreach (IDissonancePlayer player in allDissonancePlayers)
                {
                    // Player ID null check
                    if (string.IsNullOrEmpty(player.PlayerId)) { continue; }

                    // Get player voice state
                    VoicePlayerState playerState = comms.FindPlayer(player.PlayerId);
                    if (playerState != null && playerState.IsSpeaking)
                    {
                        float distance = Mathf.Max(1f, Vector3.Distance(transform.position, player.Position));
                        float loudness = playerState.Amplitude;
                        loudness *= (-Mathf.Pow(distance - 1, 2) / (sensitivity * sensitivity)) + 1;
                        loudness = Mathf.Clamp01(loudness * sensitivity);
                        SourceData.Add(new AudioSourceData(playerState.Name, loudness, player.Position));

#if UNITY_EDITOR
                        Vector3 dir = (player.Position - transform.position);
                        Debug.DrawRay(transform.position, dir * loudness, Color.white, SampleUpdateStep, true);
#endif
                    }
                }
                SourceData.OrderByDescending(sourceData => sourceData.Loudness);
            }
        }
    }
}
