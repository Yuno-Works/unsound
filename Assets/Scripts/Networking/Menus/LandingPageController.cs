using UnityEngine;

namespace SilverDogGames.Mirror.Lobby
{
    public class LandingPageController : MonoBehaviour
    {
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

        public void SetView ( bool state )
        {
            m_landingPagePanel.SetActive ( state );
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
