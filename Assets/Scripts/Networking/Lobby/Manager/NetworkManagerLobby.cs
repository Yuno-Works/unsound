using Mirror;
using Steamworks;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilverDogGames.Mirror.Lobby
{
    public class NetworkManagerLobby : NetworkManager
    {
        public static event Action OnClientStarted;
        public static event Action OnClientStopped;
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<AsyncOperation> OnGameStarted;
        public static event Action<NetworkConnection> OnClientSceneReady;
        public static event Action OnGameReady;

        public List<NetworkRoomPlayer> RoomPlayers => roomPlayers;
        public List<NetworkGamePlayer> GamePlayers => gamePlayers;

        [SerializeField]
        private List<NetworkRoomPlayer> roomPlayers = new List<NetworkRoomPlayer> ();
        [SerializeField]
        private List<NetworkGamePlayer> gamePlayers = new List<NetworkGamePlayer> ();

        [SerializeField]
        private int m_minPlayers = 1;
        [Scene]
        [SerializeField]
        private string m_menuScene = string.Empty;
        [Scene]
        [SerializeField]
        private string m_gameScene = string.Empty;

        [Header ( "Room" )]
        [SerializeField]
        private NetworkRoomPlayer m_roomPlayerPrefab = null;
        [Header ( "Game" )]
        [SerializeField]
        private NetworkGamePlayer m_gamePlayerPrefab = null;
        [SerializeField]
        private PlayerSpawnSystem m_playerSpawnSystemPrefab = null;
        [SerializeField]
        private float m_startGameDelay = 0f;

        #region Overrides

        public override void ServerChangeScene ( string newSceneName )
        {
            // Check menu scene
            if ( SceneManager.GetActiveScene ().path == m_menuScene )
            {
                // Replace room players with game players
                for ( int i = RoomPlayers.Count - 1; i >= 0; i-- )
                {
                    NetworkConnection conn = RoomPlayers [ i ].connectionToClient;
                    NetworkGamePlayer gamePlayerInstance = Instantiate ( m_gamePlayerPrefab );
                    gamePlayerInstance.SetDisplayName ( RoomPlayers [ i ].DisplayName );

                    // Destroy connection room player
                    NetworkServer.Destroy ( conn.identity.gameObject );
                    NetworkServer.ReplacePlayerForConnection ( conn, gamePlayerInstance.gameObject, true );
                }
            }

            base.ServerChangeScene ( newSceneName );
        }

        #region Callbacks

        public override void OnStartServer () => spawnPrefabs.AddRange ( Resources.LoadAll<GameObject> ( "SpawnablePrefabs" ) );

        public override void OnStartClient ()
        {
            GameObject [] spawnablePrefabs = Resources.LoadAll<GameObject> ( "SpawnablePrefabs" );

            foreach ( GameObject prefab in spawnablePrefabs )
            {
                NetworkClient.RegisterPrefab ( prefab );
            }

            OnClientStarted?.Invoke ();
        }

        public override void OnStopClient ()
        {
            OnClientStopped?.Invoke ();
        }

        public override void OnClientConnect ( NetworkConnection conn )
        {
            base.OnClientConnect ( conn );

            OnClientConnected?.Invoke ();
        }

        public override void OnClientDisconnect ( NetworkConnection conn )
        {
            base.OnClientDisconnect ( conn );

            OnClientDisconnected?.Invoke ();
        }

        public override void OnServerConnect ( NetworkConnection conn )
        {
            if ( numPlayers >= maxConnections )
            {
                conn.Disconnect ();
                return;
            }

            if ( SceneManager.GetActiveScene ().path != m_menuScene )
            {
                conn.Disconnect ();
                return;
            }
        }

        public override void OnServerAddPlayer ( NetworkConnection conn )
        {
            if ( SceneManager.GetActiveScene ().path == m_menuScene && SteamManager.Initialized )
            {
                bool isLeader = RoomPlayers.Count == 0;
                CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex ( SteamLobby.LobbyId, numPlayers - 1 );

                // Add room player instance
                NetworkRoomPlayer roomPlayerInstance = Instantiate ( m_roomPlayerPrefab );
                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection ( conn, roomPlayerInstance.gameObject );

                // Add Steam info
                LobbyPlayerInfoHandler lobbyPlayerInfo = conn.identity.GetComponent<LobbyPlayerInfoHandler> ();
                lobbyPlayerInfo.SetSteamId ( steamId.m_SteamID );
            }
        }

        public override void OnServerDisconnect ( NetworkConnection conn )
        {
            if ( conn.identity != null )
            {
                NetworkRoomPlayer player = conn.identity.GetComponent<NetworkRoomPlayer> ();

                RoomPlayers.Remove ( player );

                NotifyPlayersOfReadyState ();
            }

            base.OnServerDisconnect ( conn );
        }

        public override void OnStopServer () => RoomPlayers.Clear ();

        /// <summary>
        /// Called on the server when the client is ready (loaded into scene).
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerReady ( NetworkConnection conn )
        {
            if ( SceneManager.GetActiveScene ().path != m_gameScene ) { return; }

            base.OnServerReady ( conn );

            // Set client ready flag to true
            NetworkGamePlayer gamePlayer = GamePlayers.FirstOrDefault ( p => p.netId == conn.identity.netId );
            if ( gamePlayer != null )
            {
                gamePlayer.IsReady = true;
            }

            // Spawn players if all ready
            if ( GamePlayers.All ( p => p.IsReady ) )
            {
                OnGameReady?.Invoke ();

                // Instantiate spawn system
                PlayerSpawnSystem playerSpawnSystemInstance = Instantiate ( m_playerSpawnSystemPrefab );
                NetworkServer.Spawn ( playerSpawnSystemInstance.gameObject );
                // Spawn players
                playerSpawnSystemInstance.SpawnPlayers ( GamePlayers.Select ( p => p.connectionToClient ) );
            }
        }

        public override void OnClientSceneChanged ( NetworkConnection conn )
        {
            if ( SceneManager.GetActiveScene ().path != m_gameScene ) { return; }

            OnClientSceneReady?.Invoke ( conn );

            base.OnClientSceneChanged ( conn );
        }

        #region Game Start-up

        /// <summary>
        /// Changes the server scene and all client's scenes to game scene immediately.
        /// </summary>
        public void StartGame ()
        {
            if ( SceneManager.GetActiveScene ().path == m_menuScene )
            {
                StartCoroutine ( StartGameCountdown ( () =>
                {
                    ServerChangeScene ( m_gameScene );
                    OnGameStarted?.Invoke ( loadingSceneAsync );
                } ) );
            }
        }

        /// <summary>
        /// Callback for when the  server starts the game. OnGameStarted is raised for each connected client after startGameDelay time delay.
        /// </summary>
        public void OnStartGameClient ()
        {
            if ( SceneManager.GetActiveScene ().path == m_menuScene )
            {
                StartCoroutine ( StartGameCountdown ( () =>
                {
                    OnGameStarted?.Invoke ( loadingSceneAsync );
                } ) );
            }
        }

        private IEnumerator StartGameCountdown ( Action callback )
        {
            yield return new WaitForSeconds ( m_startGameDelay );
            callback?.Invoke ();
        }

        #endregion

        #endregion

        #endregion

        public void NotifyPlayersOfReadyState ()
        {
            foreach ( NetworkRoomPlayer player in RoomPlayers )
            {
                player.OnClientReady ();
            }
        }

        public bool IsReadyToStart ()
        {
            if ( numPlayers < m_minPlayers ) { return false; }

            // Check ready status all
            return RoomPlayers.All ( p => p.IsReady );
        }
    }
}
