using UnityEngine;
using Mirror;

namespace SilverDogGames
{
    using SilverDogGames.Mirror.Lobby;
    using Dissonance;

    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar]
        public bool IsReady;
        [SyncVar]
        private string m_displayName = "Loading";

        private NetworkManagerLobby Room
        {
            get
            {
                if ( m_room != null ) { return m_room; }
                return m_room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }
        private NetworkManagerLobby m_room;

        #region Initialization

        public void Initialize ( string displayName )
        {
            SetDisplayName ( displayName );
        }

        #endregion

        #region Callbacks

        public override void OnStartClient ()
        {
            DontDestroyOnLoad ( gameObject );

            Room.GamePlayers.Add ( this );
        }

        public override void OnStopClient ()
        {
            Room.GamePlayers.Remove ( this );
        }

        #endregion

        [Server]
        private void SetDisplayName ( string displayName ) => m_displayName = displayName;
    }
}
