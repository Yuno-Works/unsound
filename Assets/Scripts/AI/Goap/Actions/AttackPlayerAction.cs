namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using SilverDogGames.AI.Goap.States;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class AttackPlayerAction : ReGoapAction<string, object>
    {
        [SerializeField] private float attackRadius = 2f;
        [SerializeField] private LayerMask attackMask;
        private AgentActionState agentActionState = null;

        protected override void Awake()
        {
            base.Awake();
            agentActionState = GetComponent<AgentActionState>();
            effects.Set("attackedPlayer", true);
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            Debug.LogFormat("[{0}] Run()", Name);
            base.Run(previous, next, settings, goalState, done, fail);

            Transform playerT = GetClosestPlayer();
            if (playerT)
                agentActionState.Attack(playerT, OnActionDone, OnActionFailure);
            else
                failCallback(this);
        }

        public override void Exit(IReGoapAction<string, object> next)
        {
            base.Exit(next);

            var worldState = agent.GetMemory().GetWorldState();
            worldState.Remove("objective");
            worldState.Remove("objectivePosition");
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            return base.CheckProceduralCondition(stackData) && stackData.settings.HasKey("objectivePosition");
        }

        public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
        {
            preconditions.Set("atPlayer", true);
            if (stackData.settings.TryGetValue("objectivePosition", out var objectivePosition))
                preconditions.Set("isAtPosition", objectivePosition);
            return preconditions;
        }

        public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
        {
            if (stackData.currentState.TryGetValue("objectivePosition", out var objectivePosition))
            {
                settings.Set("objectivePosition", objectivePosition);
                return base.GetSettings(stackData);
            }
            return new List<ReGoapState<string, object>>();
        }

        public override float GetCost(GoapActionStackData<string, object> stackData)
        {
            var distance = 0.0f;
            if (stackData.settings.TryGetValue("objectivePosition", out object objectivePosition)
                && stackData.currentState.TryGetValue("isAtPosition", out object isAtPosition))
            {
                if (objectivePosition is Vector3 p && isAtPosition is Vector3 r)
                    distance = (p - r).magnitude;
            }
            return base.GetCost(stackData) + Cost + distance;
        }

        protected virtual void OnActionFailure()
        {
            failCallback(this);
        }

        protected virtual void OnActionDone()
        {
            doneCallback(this);
        }

        /// <summary>
        /// Returns closest player transform within the attack radius, or null.
        /// </summary>
        /// <returns>Transform of closest player, or null.</returns>
        private Transform GetClosestPlayer()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, attackRadius, attackMask);
            return cols.Select(c => c.transform).GetClosest(transform);
        }
    }
}
