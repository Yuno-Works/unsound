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
            NetworkManagerLobby.OnClientReady += OnClientReadyCallback;
            NetworkManagerLobby.OnGameReady += OnGameReadyCallback;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnClientReady -= OnClientReadyCallback;
            NetworkManagerLobby.OnGameReady -= OnGameReadyCallback;
        }

        private void OnClientReadyCallback ( NetworkConnection conn )
        {
            Debug.Log ( $"OnClientReadyCallback()" );
            OnClientReady?.Invoke ( conn );
        }
        private void OnGameReadyCallback () => OnGameReady?.Invoke ();
    }
}
