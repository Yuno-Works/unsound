namespace SilverDogGames.AI.Goap.Sensors
{
    using ReGoap.Core;
    using ReGoap.Unity;
    using UnityEngine;
    using System.Collections.Generic;

    public class SightSensor : ReGoapSensor<string, object>
    {
        public float ViewRadius { get => viewRadius; set => viewRadius = value; }
        public float ViewAngle { get => viewAngle; set => viewAngle = value; }

        [SerializeField] private float viewRadius = 10f;
        [Range(0, 360)]
        [SerializeField] private float viewAngle = 160f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private List<Transform> targets = new List<Transform>();

        public override void Init(IReGoapMemory<string, object> memory)
        {
            base.Init(memory);
            targets = new List<Transform>();
        }

        public override void UpdateSensor()
        {
            FindVisibleTargets();
        }

        /// <summary>
        /// Get the direction of an angle from the forward viewing direction.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns></returns>
        public Vector2 DirFromAngle(float angle, bool isGlobal)
        {
            if (!isGlobal)
            {
                angle += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        private void FindVisibleTargets()
        {
            targets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
                {
                    float distToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget))
                    {
                        targets.Add(target);
                    }
                }
            }
        }
    }
}
