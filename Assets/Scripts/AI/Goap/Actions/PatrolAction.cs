using UnityEngine;
using UnityEngine.AI;

namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using System;

    public class PatrolAction : ReGoapAction<string, object>
    {
        [SerializeField] private float maxWaypointDistance = 20f;

        protected override void Awake()
        {
            base.Awake();
            effects.Set("patrol", true);
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);

            Debug.LogFormat("[{0}] Run()", Name);
            doneCallback(this);
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            Vector3? waypoint = GetWaypoint();
            if (waypoint.HasValue)
            {
                var worldState = agent.GetMemory().GetWorldState();
                worldState.Set("objectivePosition", waypoint);
            }
            return waypoint.HasValue;
        }

        public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
        {
            if (stackData.currentState.TryGetValue("objectivePosition", out var objectivePosition))
            {
                preconditions.Set("isAtPosition", objectivePosition);
            }
            return base.GetPreconditions(stackData);
        }

        protected virtual void OnFailureMovement()
        {
            failCallback(this);
        }

        protected virtual void OnDoneMovement()
        {
            doneCallback(this);
        }

        private Vector3? GetWaypoint()
        {
            Vector3 randomDirection = VectorExtensions.RandomVector() * maxWaypointDistance;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maxWaypointDistance, 1))
            {
                return hit.position;
            }
            return null;
        }
    }
}
