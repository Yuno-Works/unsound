using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Mirror;

namespace SilverDogGames.Networking.NavAgent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NetworkNavAgent : NetworkBehaviour
    {
        [SerializeField, ReadOnly] private NavMeshAgent navMeshAgent = null;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (!isServer)
            {
                navMeshAgent.enabled = false;
                enabled = false;
            }
        }

        public void SetSpeed(float value)
        {
            if (isServer)
            {
                navMeshAgent.speed = value;
            }
        }

        public void SetTarget(Vector3 position)
        {
            if (isServer)
            {
                navMeshAgent.SetDestination(position);
            }
        }

        public void Stop()
        {
            if (isServer)
            {
                navMeshAgent.SetDestination(transform.position);
                navMeshAgent.ResetPath();
            }
        }
    }
}
