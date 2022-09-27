namespace SilverDogGames.Networking.FSM
{
    using HFPS.Player;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ImpairedState : BaseState
    {
        [SerializeField] private PlayerController playerController = null;
        [SerializeField] private AudioClip debugAudioClip = null;
        [SerializeField] private AudioSource debugAudioSource = null;

        public override void Init(StateMachine stateMachine)
        {
            Name = "Impaired";
            debugAudioSource = GetComponent<AudioSource>();
            base.Init(stateMachine);
        }
        public override void Enter()
        {
            base.Enter();
            playerController.isControllable = false;
            playerController.CharacterControl.enabled = false;

            // FX
            debugAudioSource.PlayOneShot(debugAudioClip);
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
        }
        public override void Exit()
        {
            base.Exit();
            playerController.isControllable = true;
            playerController.CharacterControl.enabled = true;
        }
    }
}
