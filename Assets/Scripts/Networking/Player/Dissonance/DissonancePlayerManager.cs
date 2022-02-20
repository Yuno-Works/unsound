using Mirror;
using System.Collections.Generic;
using UnityEngine;
using DFSW;
using Sirenix.Serialization;

namespace SilverDogGames
{
    using Dissonance;

    internal class DissonancePlayerManager : SingletonMonoBehaviour<DissonancePlayerManager>
    {
        [SerializeField]
        private DissonanceComms comms = null;
        [OdinSerialize]
        private readonly Dictionary<string, DissonancePlayer> players = new Dictionary<string, DissonancePlayer> ();

        public void OnEnable ()
        {
            comms.OnPlayerJoinedSession += OnPlayerJoined;
            comms.OnPlayerLeftSession += OnPlayerLeft;
            PlayerSpawnSystem.OnSpawnPlayer += SetupLocalPlayer;
        }

        public void OnDisable ()
        {
            comms.OnPlayerJoinedSession -= OnPlayerJoined;
            comms.OnPlayerLeftSession -= OnPlayerLeft;
            PlayerSpawnSystem.OnSpawnPlayer -= SetupLocalPlayer;
        }

        private void OnPlayerJoined ( VoicePlayerState player )
        {
            Debug.Log ( $"DissonancePlayerManager.OnPlayerJoined () - {player.Name}" );
            if ( player.Tracker is DissonancePlayer dissonancePlayer )
            {
                players.Add ( player.Name, dissonancePlayer );
            }
        }

        private void OnPlayerLeft ( VoicePlayerState player )
        {
            Debug.Log ( $"DissonancePlayerManager.OnPlayerLeft () - {player.Name}" );
            if ( players.TryGetValue ( player.Name, out _ ) )
            {
                players.Remove ( player.Name );
            }
        }

        private void SetupLocalPlayer ( object sender, NetworkConnection conn )
        {
            Debug.Log ( $"DissonancePlayerManager.SetupLocalPlayer () - {comms.LocalPlayerName}" );
            DissonancePlayer dissonancePlayer = conn.identity.GetComponent<DissonancePlayer> ();
            dissonancePlayer.Setup ( comms.LocalPlayerName );
        }
    }
}
