using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkNavAgent : NetworkBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private Transform currentTarget = null;
    [SerializeField] private List<Collider> targets = new List<Collider>();
    [SerializeField] private float chaseRadius = 10f;

    private NavMeshAgent navMeshAgent = null;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            FindTargets();
            GetClosestTarget();
            Chase();
        }
    }

    private void FindTargets()
    {
        var colliders = Physics.OverlapSphere(transform.position, chaseRadius, targetLayerMask);
        if (colliders != null)
        {
            // Add new targets
            foreach (var collider in colliders)
            {
                if (!targets.Contains(collider))
                {
                    targets.Add(collider);
                }
            }
            // Remove invalid targets
            var tempTargets = new List<Collider>(colliders);
            foreach (var target in targets)
            {
                if (!tempTargets.Contains(target))
                {
                    tempTargets.Remove(target);
                }
            }
            targets = new List<Collider>(tempTargets);
        }
    }

    private void GetClosestTarget()
    {
        if (targets?.Count > 0)
        {
            Transform closest = null;
            float closestDist = chaseRadius;
            foreach (var target in targets)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < closestDist)
                {
                    closest = target.transform;
                    closestDist = dist;
                }
            }
            currentTarget = closest;
        }
    }

    private void Chase()
    {
        if (currentTarget != null)
        {
            navMeshAgent.SetDestination(currentTarget.position);
        }
    }
}
