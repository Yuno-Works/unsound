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
            if (worldState.TryGetValue("visibleTargets", out var targets))
            {
                switch (targets)
                {
                    case Transform[] targetArray:
                        Transform target = GetClosestTarget(targetArray);
                        worldState.Set("playerLocated", target != null);
                        worldState.Set("objective", target);
                        if (target != null)
                        {
                            worldState.Set("objectivePosition", target.position);
                        }
                        break;
                    case Vector3[] positionArray:
                        Vector3? targetPosition = GetClosestTarget(positionArray);
                        worldState.Set("playerLocated", targetPosition.HasValue);
                        if (targetPosition.HasValue)
                        {
                            worldState.Set("objectivePosition", targetPosition.Value);
                        }
                        break;
                    default:
                        break;
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
        private Vector3? GetClosestTarget(Vector3[] targets)
        {
            if (targets == null || targets.Length == 0) return null;
            Vector3 closest = targets[0];
            float bestDist = Vector3.Distance(closest, transform.position);
            foreach (Vector3 target in targets)
            {
                float dist = Vector3.Distance(target, transform.position);
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
