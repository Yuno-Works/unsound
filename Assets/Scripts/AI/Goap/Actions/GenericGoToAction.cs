using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using SilverDogGames.AI.Goap.States;
    using System;

    [RequireComponent(typeof(AgentGoToState))]
    public class GenericGoToAction : ReGoapAction<string, object>
    {
        private AgentGoToState agentGoTo = null;

        protected override void Awake()
        {
            base.Awake();

            agentGoTo = GetComponent<AgentGoToState>();
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);

            if (settings.TryGetValue("objectivePosition", out var v))
                agentGoTo.GoTo((Vector3)v, OnDoneMovement, OnFailureMovement);
            else
                failCallback(this);
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            return base.CheckProceduralCondition(stackData) && stackData.settings.HasKey("objectivePosition");
        }

        public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
        {
            if (stackData.settings.TryGetValue("objectivePosition", out var objectivePosition))
            {
                effects.Set("isAtPosition", objectivePosition);
                if (stackData.settings.HasKey("reconcilePosition"))
                    effects.Set("reconcilePosition", true);
            }
            else
            {
                effects.Clear();
            }
            return base.GetEffects(stackData);
        }

        public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
        {
            if (stackData.currentState.TryGetValue("objective", out var objective))
            {
                settings.Set("objective", objective);
            }
            if (stackData.currentState.TryGetValue("objectivePosition", out var objectivePosition))
            {
                settings.Set("objectivePosition", objectivePosition);
            }
            return base.GetSettings(stackData);
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

        protected virtual void OnFailureMovement()
        {
            failCallback(this);
        }

        protected virtual void OnDoneMovement()
        {
            var worldState = agent.GetMemory().GetWorldState();
            worldState.Remove("objective");
            worldState.Remove("objectivePosition");
            doneCallback(this);
        }
    }
}
