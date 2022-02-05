using UnityEngine;
using DearVR;
using Mirror;

public class DearVRPlayerController : NetworkBehaviour
{
    [Header ( "References" )]
    [SerializeField]
    private AudioListener m_audioListener = null;

    #region Initialization

    public override void OnStartClient ()
    {
        if ( isLocalPlayer )
        {
            // Assign AudioListener to DearVRManager
            DearVRManager.DearListener = m_audioListener;
            Debug.Log ( $"DearVRManager.DearListener={DearVRManager.DearListener}" );
        }

        base.OnStartClient ();
    }

    #endregion
}
