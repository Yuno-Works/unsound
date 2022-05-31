namespace SilverDogGames.AI.Goap.Sensors
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using UnityEngine;

    public class ProximitySensor : ReGoapSensor<string, object>
    {
        public override void Init(IReGoapMemory<string, object> memory)
        {
            base.Init(memory);
        }

        public override void UpdateSensor()
        {
            var worldState = memory.GetWorldState();
            if (worldState.TryGetValue("visibleTargets", out var targets) && targets is Transform[] targetArray)
            {
                Transform target = GetClosestTarget(targetArray);
                worldState.Set("playerLocated", target != null);
                worldState.Set("objective", target);
                if (target != null)
                {
                    worldState.Set("objectivePosition", target.position);
                }
            }
        }
        private Transform GetClosestTarget(Transform[] targets)
        {
            if (targets == null || targets.Length == 0) return null;
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
