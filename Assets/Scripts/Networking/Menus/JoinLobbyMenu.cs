using UnityEngine;
using UnityEngine.UI;

namespace SilverDogGames.Mirror.Lobby
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField]
        private NetworkManagerLobby m_networkManager = null;

        [Header ( "UI" )]
        [SerializeField]
        private GameObject m_landingPagePanel = null;
        [SerializeField]
        private InputField m_ipAddressInputField = null;
        [SerializeField]
        private Button m_joinButton = null;

        private void OnEnable ()
        {
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby ()
        {
            string ipAddress = m_ipAddressInputField.text;

            m_networkManager.networkAddress = ipAddress;
            m_networkManager.StartClient ();

            m_joinButton.interactable = false;
        }

        private void HandleClientConnected ()
        {
            m_joinButton.interactable = true;

            gameObject.SetActive ( false );
            m_landingPagePanel.SetActive ( false );
        }

        private void HandleClientDisconnected ()
        {
            m_joinButton.interactable = true;
        }
    }
}
