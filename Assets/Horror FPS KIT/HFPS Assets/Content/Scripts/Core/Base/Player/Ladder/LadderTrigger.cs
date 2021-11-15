using UnityEngine;
using HFPS.Player;
using System.Linq;

namespace HFPS.Systems
{
    public class LadderTrigger : MonoBehaviour
    {
        private Collider colliderSelf;
        private PlayerController Player
        {
            get
            {
                if ( player == null )
                {
                    return player = FindObjectsOfType<PlayerController> ().FirstOrDefault ( p => p.isLocalPlayer );
                }
                return player;
            }
            set
            {
                player = value;
            }
        }
        private PlayerController player;
        private MouseLook MouseLook
        {
            get
            {
                if ( mouseLook == null )
                {
                    return mouseLook = ScriptManager.HasReference ? ScriptManager.Instance.GetComponent<MouseLook> () : null;
                }
                return mouseLook;
            }
            set
            {
                mouseLook = value;
            }
        }
        private MouseLook mouseLook;

        [Header("Mouse Lerp")]
        public Vector2 LadderLook;

        [Header("Ladder")]
        public Transform CenterDown;
        public Transform CenterUp;
        public Transform UpFinishPosition;
        [Space(7)]
        public LadderEvent UpFinishTrigger;
        public LadderEvent DownFinishTrigger;
        [Space(7)]
        public float UpDistance;
        public bool IsPlayerUp;

        private bool onLadder;

        void Awake()
        {
            colliderSelf = GetComponentInChildren<Collider>();
        }

        void Update()
        {
            if (!Player) return;

            if (Vector3.Distance(Player.transform.position, UpFinishTrigger.transform.position) > UpDistance)
            {
                IsPlayerUp = false;
            }
            else
            {
                IsPlayerUp = true;
            }

            onLadder = Player.ladderReady;
            colliderSelf.enabled = Player.movementState != PlayerController.MovementState.Ladder;

            UpFinishTrigger.blockTrigger = !Player.ladderReady;
            DownFinishTrigger.blockTrigger = !Player.ladderReady;
        }

        public void UseObject()
        {
            if (!Player) return;

            Vector2 rotation = LadderLook;
            rotation.x -= MouseLook.playerOriginalRotation.eulerAngles.y;

            if (!IsPlayerUp)
            {
                Player?.CmdUseLadder(CenterDown, rotation, true);
            }
            else
            {
                Player?.CmdUseLadder(CenterUp, rotation, false);
            }
        }

        public void OnClimbFinish(bool finishUp)
        {
            if (!Player) return;

            if (onLadder)
            {
                if (finishUp)
                {
                    Player.LerpPlayerLadder(UpFinishPosition.position);
                }
                else
                {
                    Player.CmdLadderExit();
                }
            }
        }

        void OnDrawGizmos()
        {
            if (CenterDown) { Gizmos.color = Color.green; Gizmos.DrawSphere(CenterDown.position, 0.05f); }
            if (CenterUp) { Gizmos.color = Color.red; Gizmos.DrawSphere(CenterUp.position, 0.05f); }
            if (UpFinishPosition) { Gizmos.color = Color.white; Gizmos.DrawSphere(UpFinishPosition.position, 0.1f); }

            if (Player)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, 0.2f);

                Vector2 rotation = LadderLook;
                rotation.x -= Player.transform.eulerAngles.y;

                Vector3 gizmoRotation = Quaternion.Euler(new Vector3(0, rotation.x, rotation.y)) * Player.transform.forward * 1f;

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, gizmoRotation);
            }
        }
    }
}