using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SilverDogGames
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField]
        private GameObject m_playerPrefab = null;

        private static List<Transform> m_spawnPoints = new List<Transform> ();

        private int m_nextIndex = 0;

        public static void AddSpawnPoint ( Transform transform )
        {
            m_spawnPoints.Add ( transform );
            m_spawnPoints = m_spawnPoints.OrderBy ( x => x.GetSiblingIndex () ).ToList ();
        }

        public static bool RemoveSpawnPoint ( Transform transform ) => m_spawnPoints.Remove ( transform );


        [Server]
        public void SpawnPlayers ( IEnumerable<NetworkConnection> connections )
        {
            foreach ( NetworkConnection conn in connections )
            {
                Transform spawnPoint = m_spawnPoints.ElementAtOrDefault ( m_nextIndex );

                if ( spawnPoint == null )
                {
                    Debug.LogError ( $"Missing spawn point for player {m_nextIndex}" );
                    return;
                }

                GameObject playerInstance = Instantiate ( m_playerPrefab, m_spawnPoints [ m_nextIndex ].position, m_spawnPoints [ m_nextIndex ].rotation );
                NetworkServer.ReplacePlayerForConnection ( conn, playerInstance );

                playerInstance.name = $"Player [{conn.identity.netId}]";

                m_nextIndex = ( m_nextIndex + 1 ) % m_spawnPoints.Count;
            }
        }
    }
}
