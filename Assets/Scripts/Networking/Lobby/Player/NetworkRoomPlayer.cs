using System.Linq;
using UnityEngine;
using Mirror;
using Steamworks;

namespace SilverDogGames.Mirror.Lobby
{
    public class NetworkRoomPlayer : NetworkBehaviour
    {
        [SyncVar ( hook = nameof ( HandleDisplayNameChanged ) )]
        public string DisplayName = "Loading";
        [SyncVar ( hook = nameof ( HandleReadyStatusChanged ) )]
        public bool IsReady = false;
        [SyncVar ( hook = nameof ( HandleGameStart ) )]
        public bool IsGameStarted = false;

        [Header ( "References" )]
        [SerializeField]
        private NetworkLobbyView m_view = null;

        public bool IsLeader
        {
            set
            {
                m_isLeader = value;
            }
        }
        private bool m_isLeader;

        private NetworkManagerLobby Room
        {
            get
            {
                if ( m_room != null ) { return m_room; }
                return m_room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }
        private NetworkManagerLobby m_room;

        private void Awake ()
        {
            // Hide view
            m_view.DisplayPanel ( false );
        }

        #region Callbacks

        public override void OnStartAuthority ()
        {
            if ( SteamManager.Initialized )
            {
                // Set room player display name
                SetDisplayName ( SteamFriends.GetPersonaName () );

                m_view.DisplayPanel ( true );
            }
            else
            {
                Debug.LogError ( $"{this} SteamManager not initialized." );
            }
        }

        public override void OnStartClient ()
        {
            Room.RoomPlayers.Add ( this );

            UpdateView ();
        }

        public override void OnStopClient ()
        {
            Room.RoomPlayers.Remove ( this );

            UpdateView ();
        }

        public override void OnStopAuthority () => m_view.DisplayPanel ( false );

        public void HandleDisplayNameChanged ( string oldValue, string newValue ) => UpdateView ();
        public void HandleReadyStatusChanged ( bool oldValue, bool newValue ) => UpdateView ();
        public void HandleGameStart ( bool oldValue, bool newValue ) => UpdateView ();

        private void UpdateView ()
        {
            if ( !hasAuthority )
            {
                NetworkRoomPlayer player = Room.RoomPlayers.FirstOrDefault ( p => p.hasAuthority );

                if ( player != null && player.hasAuthority )
                {
                    player.UpdateView ();
                }

                return;
            }

            m_view.UpdateView ( Room.RoomPlayers, IsReady, IsGameStarted );
        }

        #endregion

        #region Listeners

        public void SetDisplayName ( string displayName ) => CmdSetDisplayName ( displayName );

        /// <summary>
        /// Activates Steam friends invite overlay.
        /// </summary>
        public void OpenSteamOverlayInviteDialog () => SteamFriends.ActivateGameOverlayInviteDialog ( SteamLobby.LobbyId );

        public void OnClientReady () => CmdOnClientReady ();

        #endregion

        #region Commands

        [Command]
        private void CmdSetDisplayName ( string displayName ) => DisplayName = displayName;

        [Command]
        private void CmdOnClientReady ()
        {
            // Client ready up
            // Invokes ready up handler
            IsReady = !IsReady;

            // Check ready status all
            if ( Room.IsReadyToStart () )
            {
                // Start game
                Room.StartGame ();
                RpcStartGameClient ();

                // Notify each connection game start event
                foreach ( NetworkRoomPlayer player in Room.RoomPlayers )
                {
                    player.IsGameStarted = true;
                }
            }
        }

        #endregion

        #region RPCs

        [ClientRpc]
        private void RpcStartGameClient () => Room.OnStartGameClient ();

        #endregion
    }
}
