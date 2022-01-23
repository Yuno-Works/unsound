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

        private NetworkManagerLobby m_room;
        private NetworkManagerLobby Room
        {
            get
            {
                if ( m_room != null ) { return m_room; }
                return m_room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        #region Callbacks

        public override void OnStartAuthority ()
        {
            if ( SteamManager.Initialized )
            {
                // Set room player display name
                CmdSetDisplayName ( SteamFriends.GetPersonaName () );

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

        public override void OnStopAuthority ()
        {
            m_view.DisplayPanel ( false );
        }

        public void HandleDisplayNameChanged ( string oldValue, string newValue ) => UpdateView ();
        public void HandleReadyStatusChanged ( bool oldValue, bool newValue ) => UpdateView ();
        public void HandleGameStart ( bool oldValue, bool newValue ) => UpdateView ();

        #endregion

        #region Listeners

        /// <summary>
        /// Activates Steam friends invite overlay.
        /// </summary>
        public void OpenSteamOverlayInviteDialog ()
        {
            SteamFriends.ActivateGameOverlayInviteDialog ( SteamLobby.LobbyId );
        }

        public void HandleReadyToStart ()
        {
            CmdStartGame ();
        }

        #endregion

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

        #region Commands

        [Command]
        public void CmdSetDisplayName ( string displayName ) => DisplayName = displayName;

        [Command]
        public void CmdReadyUp () => IsReady = !IsReady;

        [Command]
        private void CmdStartGame ()
        {
            // Check ready status all
            if ( Room.IsReadyToStart () == false ) { return; }

            // Start game
            Room.StartGame ();
            RpcStartGameClient ();

            // Notify each connection game start event
            foreach ( NetworkRoomPlayer player in Room.RoomPlayers )
            {
                player.IsGameStarted = true;
            }
        }

        #endregion

        #region RPCs

        [ClientRpc]
        private void RpcStartGameClient () => Room.OnStartGameClient ();

        #endregion
    }
}
