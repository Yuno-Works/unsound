using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using System;

    [RequireComponent(typeof(GenericGoToAction))]
    public class PatrolAction : ReGoapAction<string, object>
    {
        [SerializeField] private float maxWaypointDistance = 20f;
        [SerializeField] private GenericGoToAction goToActionComponent = null;

        protected override void Awake()
        {
            base.Awake();

            goToActionComponent = GetComponent<GenericGoToAction>();
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);

            Debug.LogFormat("[{0}] Run()", Name);
            BeginPatrol();
        }

        public override void Exit(IReGoapAction<string, object> next)
        {
            base.Exit(next);

            var worldState = agent.GetMemory().GetWorldState();
            worldState.Set("patrol", false);
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            return base.CheckProceduralCondition(stackData) &&
                stackData.settings.TryGetValue("playerLocated", out var playerLocated) &&
                playerLocated is bool bPlayerLocated && bPlayerLocated;
        }

        public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
        {
            return base.GetPreconditions(stackData);
        }

        public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
        {
            effects.Set("patrol", true);
            return base.GetEffects(stackData);
        }

        public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
        {
            //if (stackData.currentState.TryGetValue("objectivePosition", out var objectivePosition))
            //{
            //    settings.Set("objectivePosition", objectivePosition);
            //}
            return base.GetSettings(stackData);
        }

        protected virtual void OnFailureMovement()
        {
            failCallback(this);
        }

        protected virtual void OnDoneMovement()
        {
            doneCallback(this);
        }

        #region Patrol Behavior

        private void BeginPatrol()
        {
            StartCoroutine(PatrolLoop());
        }

        private IEnumerator PatrolLoop()
        {
            Vector3 waypoint;
            var worldState = agent.GetMemory().GetWorldState();
            while (enabled)
            {
                waypoint = GetWaypoint();
                worldState.Set("objectivePosition", waypoint);

                // Wait until GoTo action finishes
                yield return new WaitUntil(() => goToActionComponent.enabled == false);
            }
            doneCallback(this);
            yield break;
        }

        private Vector3 GetWaypoint()
        {
            Vector3 randomDirection = VectorExtensions.RandomVector() * maxWaypointDistance;
            randomDirection += transform.position;
            Vector3 finalPosition = transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maxWaypointDistance, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        #endregion
    }
}
