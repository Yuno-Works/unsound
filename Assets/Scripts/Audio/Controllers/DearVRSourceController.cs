namespace SilverDogGames.Audio
{
    using DearVR;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(DearVRSource))]
    public class DearVRSourceController : MonoBehaviour
    {
        [SerializeField] private LayerMask ReverbZoneMask;
        private DearVRSource source = null;

        private void Awake()
        {
            source = GetComponent<DearVRSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckLayerMask(other.gameObject))
            {
                Debug.LogFormat("entered reverb zone: {0}", other.gameObject);
                DearVRReverbZone reverbZone;
                if ((reverbZone = other.GetComponent<DearVRReverbZone>()) != null)
                {
                    List<DearVRSerializedReverb> newSends = new List<DearVRSerializedReverb>(1);
                    foreach (var send in reverbZone.ReverbSends)
                    {
                        if (!ContainsReverbSend(send.roomIndex))
                        {
                            newSends.Add(send);
                        }
                    }
                    source.SetReverbSends(newSends.ToArray());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CheckLayerMask(other.gameObject))
            {
                Debug.LogFormat("exited reverb zone: {0}", other.gameObject);
                DearVRReverbZone reverbZone;
                if ((reverbZone = other.GetComponent<DearVRReverbZone>()) != null)
                {
                    List<DearVRSerializedReverb> sourceSends = new List<DearVRSerializedReverb>(source.GetReverbSendList());
                    foreach (var send in reverbZone.ReverbSends)
                    {
                        if (ContainsReverbSend(send.roomIndex))
                        {
                            sourceSends.Remove(send);
                        }
                    }
                    source.SetReverbSends(sourceSends.ToArray());
                }
            }
        }

        private bool ContainsReverbSend(int reverbSendId)
        {
            if (reverbSendId <= 0)
            {
                return false;
            }
            return source.GetReverbSendList().Any(r => r.roomIndex == reverbSendId);
        }

        /// <summary>
        /// Is this object's layer in the layer mask?
        /// </summary>
        /// <param name="other">Object to check against.</param>
        /// <returns><c>True</c> if <paramref name="other"/> layer is in the layer mask.</returns>
        private bool CheckLayerMask(GameObject other)
        {
            return (ReverbZoneMask.value & (1 << other.layer)) > 0;
        }
    }
}
