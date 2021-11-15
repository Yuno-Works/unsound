using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    #region Members

    [Header ( "Render Targets" )]
    [SerializeField]
    private GameObject [] m_renderTargets = null;
    [SerializeField]
    private string m_visibleLayer;

    [Header ( "Disable Remote Objects/Components" )]
    [SerializeField]
    private GameObject [] m_remoteDisableObjects;
    [SerializeField]
    private MonoBehaviour [] m_remoteDisableComponents;
    [SerializeField]
    private Collider [] m_disableColliders;
    [SerializeField]
    private AudioListener m_disableAudioListener;

    #endregion

    #region Initialization

    private void Start ()
    {
        if ( !isLocalPlayer )
        {
            // Disable remote player GameObjects
            foreach ( GameObject gameObj in m_remoteDisableObjects )
            {
                gameObj.SetActive ( false );
            }
            // Disable remote player MonoBehaviours
            foreach ( MonoBehaviour component in m_remoteDisableComponents )
            {
                component.enabled = false;
            }
            // Disable remote player Colliders
            foreach ( Collider collider in m_disableColliders )
            {
                collider.enabled = false;
            }
            // Disable remote player AudioListener
            m_disableAudioListener.enabled = false;

            // Switch layers to visible
            foreach ( GameObject target in m_renderTargets )
            {
                target.layer = LayerMask.NameToLayer ( m_visibleLayer );
            }
        }
    }

    #endregion
}
