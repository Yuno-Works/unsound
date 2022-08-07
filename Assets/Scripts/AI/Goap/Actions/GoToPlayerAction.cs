using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using System;

    public class GoToPlayerAction : GenericGoToAction
    {
        [SerializeField] private float maxPositionDelta = 1f;
        [SerializeField, ReadOnly] private Vector3 lastPlayerPosition;

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);

            Debug.LogErrorFormat("[{0}] Run()", Name);
            if (settings.TryGetValue("objectivePosition", out var v))
            {
                lastPlayerPosition = (Vector3)v;
            }
            else
                failCallback(this);
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            return base.CheckProceduralCondition(stackData) && stackData.settings.TryGetValue("playerLocated", out var playerLocated) && (bool)playerLocated == true;
        }

        public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
        {
            if (stackData.settings.HasKey("objectivePosition"))
            {
                effects.Set("atPlayer", true);
            }
            return base.GetEffects(stackData);
        }

        public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
        {
            if (stackData.currentState.TryGetValue("playerLocated", out var playerLocated))
            {
                settings.Set("playerLocated", playerLocated);
            }
            return base.GetSettings(stackData);
        }

        private void FixedUpdate()
        {
            UpdateGoToLoop();
        }

        private void UpdateGoToLoop()
        {
            var worldState = agent.GetMemory().GetWorldState();
            if (worldState.TryGetValue("objectivePosition", out var objPos) && objPos is Vector3 objectivePosition)
            {
                float delta = (lastPlayerPosition - objectivePosition).magnitude;
                if (delta > maxPositionDelta)
                {
                    OnFailureMovement();
                }
            }
        }
    }
}
