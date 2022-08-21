namespace SilverDogGames.Audio
{
    using DearVR;
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DearVRReverbZone : MonoBehaviour
    {
        public DearVRSerializedReverb[] ReverbSends => reverbSends;

        [SerializeField] private DearVRSerializedReverb[] reverbSends = null;

        private BoxCollider m_collider = null;
        private Rigidbody m_rigidbody = null;

        private void Awake()
        {
            m_collider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();

            m_collider.isTrigger = true;
            m_rigidbody.isKinematic = true;
            m_rigidbody.useGravity = false;
        }
    }
}
