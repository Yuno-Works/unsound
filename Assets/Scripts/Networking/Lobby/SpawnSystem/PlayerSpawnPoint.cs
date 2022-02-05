using UnityEngine;

namespace SilverDogGames.Mirror.Lobby
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake () => PlayerSpawnSystem.AddSpawnPoint ( transform );
        private void OnDestroy () => PlayerSpawnSystem.RemoveSpawnPoint ( transform );

        private void OnDrawGizmos ()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere ( transform.position, 0.5f );
            Gizmos.color = Color.white;
            Gizmos.DrawLine ( transform.position, transform.position + transform.forward * 1f );
        }
    }
}
