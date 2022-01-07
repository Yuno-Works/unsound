using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SilverDogGames.Mirror.Lobby
{
    public class NetworkLobbyView : MonoBehaviour
    {
        private const int MAX_PLAYER_COUNT = 4;
        private const string WAITING_FOR_PLAYER = "Waiting For Player...";
        private const string NOT_READY = "Not Ready";
        private const string READY = "Ready";
        private const string UNREADY = "Unready";

        [Header ( "UI" )]
        [SerializeField]
        private GameObject m_lobbyUI = null;
        [SerializeField]
        private Text [] m_playerNameTexts = new Text [ MAX_PLAYER_COUNT ];
        [SerializeField]
        private Text [] m_playerReadyTexts = new Text [ MAX_PLAYER_COUNT ];
        [SerializeField]
        private Button m_readyButton = null;
        [SerializeField]
        private Text m_readyButtonText = null;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void DisplayPanel ( bool value )
        {
            m_lobbyUI.SetActive ( value );
        }

        public void UpdateView ( List<NetworkRoomPlayer> players, bool isReady, bool isGameStarted )
        {
            // Initialize player UI (DisplayName, Ready status)
            for ( int i = 0; i < m_playerNameTexts.Length; i++ )
            {
                m_playerNameTexts [ i ].text = WAITING_FOR_PLAYER;
                m_playerReadyTexts [ i ].text = string.Empty;
            }

            // Set player UI (DisplayName, Ready status)
            for ( int i = 0; i < players.Count; i++ )
            {
                m_playerNameTexts [ i ].text = players [ i ].DisplayName;
                m_playerReadyTexts [ i ].text = players [ i ].IsReady ? READY : NOT_READY;
            }

            // Ready up button (state, text)
            m_readyButton.interactable = !isGameStarted;
            m_readyButtonText.text = isReady ? UNREADY : READY;

            // Lobby view state
            // Hide when game is started
            m_lobbyUI.SetActive ( !isGameStarted );
        }
    }
}
