using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace SilverDogGames.Mirror.Lobby
{
    public class NetworkManagerListener : MonoBehaviour
    {
        public UnityEvent<NetworkConnection> OnClientReady = null;
        public UnityEvent OnGameReady = null;


        private void OnEnable ()
        {
            NetworkManagerLobby.OnClientSceneReady += NetworkManagerLobby_OnClientSceneReady;
            NetworkManagerLobby.OnGameReady += NetworkManagerLobby_OnGameReady;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnClientSceneReady -= NetworkManagerLobby_OnClientSceneReady;
            NetworkManagerLobby.OnGameReady -= NetworkManagerLobby_OnGameReady;
        }

        private void NetworkManagerLobby_OnClientSceneReady ( NetworkConnection conn ) => OnClientReady?.Invoke ( conn );
        private void NetworkManagerLobby_OnGameReady () => OnGameReady?.Invoke ();
    }
}
