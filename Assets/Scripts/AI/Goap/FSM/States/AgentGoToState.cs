using System;
using UnityEngine;

namespace SilverDogGames.AI.Goap.States
{
    using SilverDogGames.Networking.NavAgent;
    using ReGoap.Unity.FSM;
    using ReGoap.Utilities;
    using Sirenix.OdinInspector;

    [RequireComponent(typeof(NetworkNavAgent))]
    public class AgentGoToState : SmState
    {
        #region Models

        private enum GoToState
        {
            Disabled, Pulsed, Active, Success, Failure
        }

        #endregion

        public float Speed => speed;
        public bool WorkInFixedUpdate => workInFixedUpdate;
        public float MinPowDistanceToObjective => minPowDistanceToObjective;
        public bool CheckForStuck => checkForStuck;
        public float StuckCheckDelay => stuckCheckDelay;
        public float MaxStuckDistance => maxStuckDistance;

        [SerializeField, ReadOnly] private NetworkNavAgent navMeshAgent = null;
        [SerializeField] private float speed = 3f;
        [SerializeField] private bool workInFixedUpdate = true;
        [SerializeField] private Vector3? objective = null;
        [SerializeField] private Transform objectiveTransform = null;
        // When the magnitude of the difference between the objective and self is <= this value then we're done
        [SerializeField] private float minPowDistanceToObjective = 0.5f;
        [SerializeField] private Action onDoneMovementCallback = null;
        [SerializeField] private Action onFailureMovementCallback = null;
        [Space]
        [SerializeField] private bool checkForStuck = true;
        [SerializeField] private float stuckCheckDelay = 1f;
        [SerializeField] private float maxStuckDistance = 0.1f;
        [SerializeField, ReadOnly] private Vector3 lastStuckCheckUpdatePosition = Vector3.zero;
        [SerializeField, ReadOnly] private float stuckCheckCooldown = 0f;
        [Space]
        [SerializeField] private GoToState currentState;

        protected override void Awake()
        {
            base.Awake();
            // Init NavMeshAgent
            navMeshAgent = GetComponent<NetworkNavAgent>();
            navMeshAgent.SetSpeed(Speed);
        }

        protected virtual float GetSpeed()
        {
            return Speed;
        }

        #region Work
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!WorkInFixedUpdate) return;
            Tick();
        }

        protected override void Update()
        {
            base.Update();
            if (WorkInFixedUpdate) return;
            Tick();
        }

        protected virtual void Tick()
        {
            var objectivePosition = objectiveTransform != null ? objectiveTransform.position : objective.GetValueOrDefault();
            MoveTo(objectivePosition);
        }

        protected virtual void MoveTo(Vector3 position)
        {
            // Set destination
            navMeshAgent.SetDestination(position);
            
            // Check min move distance
            var delta = position - transform.position;
            if (delta.sqrMagnitude <= MinPowDistanceToObjective)
            {
                currentState = GoToState.Success;
            }
            if (CheckForStuck && CheckIfStuck())
            {
                currentState = GoToState.Failure;
                Exit();
            }
        }

        private bool CheckIfStuck()
        {
            if (Time.time > stuckCheckCooldown)
            {
                stuckCheckCooldown = Time.time + StuckCheckDelay;
                if ((lastStuckCheckUpdatePosition - transform.position).magnitude < MaxStuckDistance)
                {
                    ReGoapLogger.Log("[AgentGoToState] '" + name + "' is stuck.");
                    return true;
                }
                lastStuckCheckUpdatePosition = transform.position;
            }
            return false;
        }

        #endregion

        #region StateHandler
        public override void Init(StateMachine stateMachine)
        {
            base.Init(stateMachine);
            var transition = new SmTransition(GetPriority(), Transition);
            var doneTransition = new SmTransition(GetPriority(), DoneTransition);
            stateMachine.GetComponent<AgentIdleState>().Transitions.Add(transition);
            Transitions.Add(doneTransition);
        }

        private Type DoneTransition(ISmState state)
        {
            if (currentState != GoToState.Active)
                return typeof(AgentIdleState);
            return null;
        }

        private Type Transition(ISmState state)
        {
            if (currentState == GoToState.Pulsed)
                return GetType();
            return null;
        }

        public void GoTo(Vector3? position, Action onDoneMovement, Action onFailureMovement)
        {
            objective = position;
            GoTo(onDoneMovement, onFailureMovement);
        }

        public void GoTo(Transform transform, Action onDoneMovement, Action onFailureMovement)
        {
            objectiveTransform = transform;
            GoTo(onDoneMovement, onFailureMovement);
        }

        void GoTo(Action onDoneMovement, Action onFailureMovement)
        {
            currentState = GoToState.Pulsed;
            onDoneMovementCallback = onDoneMovement;
            onFailureMovementCallback = onFailureMovement;
        }

        public override void Enter()
        {
            base.Enter();
            currentState = GoToState.Active;
        }

        public override void Exit()
        {
            base.Exit();
            navMeshAgent.Stop();
            if (currentState == GoToState.Success)
                onDoneMovementCallback();
            else
                onFailureMovementCallback();
        }
        #endregion
    }
}
