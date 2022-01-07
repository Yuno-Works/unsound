using UnityEngine;

namespace SilverDogGames.Mirror.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private NetworkManagerLobby m_networkManager = null;

        [Header ( "UI" )]
        [SerializeField]
        private GameObject m_landingPagePanel = null;

        private void OnEnable ()
        {
            NetworkManagerLobby.OnClientStopped += HandleClientStopped;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnClientStopped -= HandleClientStopped;
        }

        public void HostLobby ()
        {
            m_networkManager.StartHost ();
            m_landingPagePanel.SetActive ( false );
        }

        private void HandleClientStopped ()
        {
            m_landingPagePanel.SetActive ( true );
        }

        public void Quit ()
        {
            Application.Quit ();
        }
    }
}
