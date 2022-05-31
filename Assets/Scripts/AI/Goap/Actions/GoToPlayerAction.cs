using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.AI.Goap.Actions
{
    using ReGoap.Core;
    using SilverDogGames.AI.Goap.States;
    using System;

    [RequireComponent(typeof(AgentGoToState))]
    public class GoToPlayerAction : GenericGoToAction
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
        {
            //Debug.LogFormat("[{0}] Run()", Name);
            base.Run(previous, next, settings, goalState, done, fail);
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
    }
}
