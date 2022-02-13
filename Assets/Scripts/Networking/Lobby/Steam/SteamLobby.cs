using System;
using UnityEngine;
using Steamworks;
using Mirror;

namespace SilverDogGames.Mirror.Lobby
{
    public class SteamLobby : MonoBehaviour
    {
        private const string HOST_ADDRESS_KEY = "HostAddress";

        public static CSteamID LobbyId { get; private set; }

        public static event EventHandler<LobbyCreated_t> OnLobbyCreated;
        public static event EventHandler<LobbyEnter_t> OnLobbyEnter;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        private CallResult<LobbyEnter_t> lobbyEnterResult;

        [Header ( "References" )]
        [SerializeField]
        private NetworkManager m_networkManager = null;

        // Start is called before the first frame update
        void Start ()
        {
            if ( !SteamManager.Initialized ) { return; }

            lobbyCreated = Callback<LobbyCreated_t>.Create ( LobbyCreated );
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create ( GameLobbyJoinRequested );
            //lobbyEntered = Callback<LobbyEnter_t>.Create ( LobbyEntered );
        }

        public void HostLobby ()
        {
            SteamMatchmaking.CreateLobby ( ELobbyType.k_ELobbyTypeFriendsOnly, m_networkManager.maxConnections );
        }

        public void ActivateSteamOverlay ()
        {
            SteamFriends.ActivateGameOverlay ( "test" );
        }

        #region Callbacks

        private void LobbyCreated ( LobbyCreated_t callback )
        {
            if ( callback.m_eResult == EResult.k_EResultOK )
            {
                LobbyId = new CSteamID ( callback.m_ulSteamIDLobby );
                m_networkManager.StartHost ();
                SteamMatchmaking.SetLobbyData ( LobbyId, HOST_ADDRESS_KEY, SteamUser.GetSteamID ().ToString () );
            }
            OnLobbyCreated?.Invoke ( this, callback );
        }

        /// <summary>
        /// Called when the user tries to join a lobby from their friends list or from an invite. The game client should attempt to connect to specified lobby when this is received.
        /// </summary>
        /// <param name="callback"></param>
        private void GameLobbyJoinRequested ( GameLobbyJoinRequested_t callback )
        {
            SteamAPICall_t result = SteamMatchmaking.JoinLobby ( callback.m_steamIDLobby );
            lobbyEnterResult = CallResult<LobbyEnter_t>.Create ( LobbyEnterCallback );
            lobbyEnterResult.Set ( result );
        }

        private void LobbyEnterCallback ( LobbyEnter_t callback, bool bIOFailure )
        {
            if ( NetworkServer.active ) { return; }
            if ( bIOFailure )
            {
                Debug.LogError ( "Failed to join lobby" );
                return;
            }

            string hostAddress = SteamMatchmaking.GetLobbyData ( new CSteamID ( callback.m_ulSteamIDLobby ), HOST_ADDRESS_KEY );
            m_networkManager.networkAddress = hostAddress;
            m_networkManager.StartClient ();
            OnLobbyEnter?.Invoke ( this, callback );
        }

        #endregion
    }
}
