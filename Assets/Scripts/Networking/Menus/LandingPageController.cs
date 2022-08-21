using UnityEngine;

namespace SilverDogGames.Mirror.Lobby
{
    public class LandingPageController : MonoBehaviour
    {
        [Header ( "UI" )]
        [SerializeField]
        private GameObject m_landingPagePanel = null;

        #region Initialization

        private void OnEnable ()
        {
            NetworkManagerLobby.OnClientConnected += NetworkManagerLobby_OnClientConnected;
            NetworkManagerLobby.OnClientStopped += NetworkManagerLobby_OnClientStopped;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnClientConnected -= NetworkManagerLobby_OnClientConnected;
            NetworkManagerLobby.OnClientStopped -= NetworkManagerLobby_OnClientStopped;
        }

        #endregion

        #region Callbacks

        private void NetworkManagerLobby_OnClientConnected () => SetView ( false );

        private void NetworkManagerLobby_OnClientStopped () => SetView ( true );

        #endregion

        public void SetView ( bool state )
        {
            m_landingPagePanel.SetActive ( state );
        }

        public void Quit ()
        {
            Application.Quit ();
        }
    }
}
