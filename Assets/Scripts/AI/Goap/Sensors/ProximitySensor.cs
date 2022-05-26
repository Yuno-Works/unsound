namespace SilverDogGames.AI.Goap.Sensors
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using UnityEngine;
    using System.Collections.Generic;

    public class ProximitySensor : ReGoapSensor<string, object>
    {
        [SerializeField] private List<Transform> targets = null;

        public override void Init(IReGoapMemory<string, object> memory)
        {
            base.Init(memory);
            targets = new List<Transform>(4);
        }

        public override void UpdateSensor()
        {
            var worldState = memory.GetWorldState();
            if (worldState.TryGetValue("visibleTargets", out var targets))
            {
                this.targets.Clear();
                this.targets.AddRange(targets as Transform[]);
                Transform target = GetClosestTarget();
                if (target != null)
                {
                    worldState.Set("objective", target);
                    worldState.Set("objectivePosition", target.position);
                }
            }
        }
        private Transform GetClosestTarget()
        {
            if (targets.Count == 0) return null;
            Transform closest = targets[0];
            float bestDist = Vector3.Distance(closest.position, transform.position);
            foreach (Transform target in targets)
            {
                float dist = Vector3.Distance(target.position, transform.position);
                if (dist < bestDist)
                {
                    closest = target;
                    bestDist = dist;
                }
            }
            return closest;
        }
    }
}
