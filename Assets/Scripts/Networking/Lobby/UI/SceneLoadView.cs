using UnityEngine;
using UnityEngine.UI;

namespace SilverDogGames.Mirror.Lobby
{
    public class SceneLoadView : MonoBehaviour
    {
        [Header ( "UI" )]
        [SerializeField]
        private GameObject m_loadingScenePanel = null;
        [SerializeField]
        private Slider m_progressSlider = null;

        private AsyncOperation m_sceneLoadOperation = null;

        private void OnEnable ()
        {
            NetworkManagerLobby.OnGameStarted += DisplaySceneLoader;
        }

        private void OnDisable ()
        {
            NetworkManagerLobby.OnGameStarted -= DisplaySceneLoader;
        }

        public void DisplaySceneLoader ( AsyncOperation asyncSceneLoad )
        {
            m_loadingScenePanel.SetActive ( true );
            m_sceneLoadOperation = asyncSceneLoad;
        }

        private void Update ()
        {
            if ( m_sceneLoadOperation != null )
            {
                m_progressSlider.value = m_sceneLoadOperation.progress;
            }
        }
    }
}
