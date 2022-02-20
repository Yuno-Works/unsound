using UnityEngine;
using Mirror;

namespace SilverDogGames
{
    using Dissonance;

    public class DissonancePlayer : NetworkBehaviour, IDissonancePlayer
    {
        public string PlayerId => _playerId;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public NetworkPlayerType Type => _isLocal ? NetworkPlayerType.Local : NetworkPlayerType.Remote;
        public bool IsTracking => _isTracking;

        [SerializeField]
        private bool _isTracking = false;
        [SerializeField]
        private bool _isLocal = false;
        [SyncVar ( hook = nameof ( SetupCallback ) )]
        private string _playerId = null;

        private DissonanceComms _comms;

        private void Awake ()
        {
            // Get local DissonanceComms object
            _comms = FindObjectOfType<DissonanceComms> ();
        }

        public override void OnStartAuthority ()
        {
            base.OnStartAuthority ();

            // Call set player name, to sync the name across all peers
            SetPlayerName ( _comms.LocalPlayerName );

            // Make sure that if the local name is changed
            // the change is synced across the network
            _comms.LocalPlayerNameChanged += SetPlayerName;
        }

        private void SetPlayerName ( string playerName ) => CmdSetPlayerId ( playerName );
        //private void SetPlayerName ( string playerName ) => _playerId = playerName;

        [Command]
        private void CmdSetPlayerId ( string playerId ) => _playerId = playerId;

        public void Setup ( string playerId )
        {
            Debug.Log ( $"DissonancePlayer.Setup () - {playerId}" );
            _playerId = playerId;
            _isLocal = true;
            _isTracking = true;
            _comms.TrackPlayerPosition ( this );
        }

        /// <summary>
        /// Invoked on the client for this DissonancePlayer when <see cref="_playerId"/> changes.
        /// </summary>
        /// <param name="_">Old playerId (unused)</param>
        /// <param name="playerId">New playerId</param>
        private void SetupCallback ( string _, string playerId )
        {
            Debug.Log ( $"DissonancePlayer.SetupCallback () {playerId} isLocalPlayer:{isLocalPlayer}" );
            _isLocal = isLocalPlayer;
            _isTracking = true;
            _comms.TrackPlayerPosition ( this );

            /*if ( !isLocalPlayer )
            {
                Debug.Log ( $"DissonancePlayer.SetupRemote () {playerId}" );
                _isLocal = false;
                _isTracking = true;
                _comms.TrackPlayerPosition ( this );
            }*/
        }

        private void OnDisable ()
        {
            if ( _comms != null && _isTracking )
                _comms.StopTracking ( this );
            _isTracking = false;
        }
    }
}
