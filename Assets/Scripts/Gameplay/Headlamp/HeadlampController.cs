using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderWire.Input;
using Mirror;

public class HeadlampController : NetworkBehaviour
{
    private const string TOGGLE_ACTION_NAME = "ToggleFlashlight";

    [Header ( "References" )]
    [SerializeField]
    private NetworkIdentity m_networkIdentity = null;
    [SerializeField]
    private Transform m_lightObject = null;
    [SerializeField]
    private Light m_localLight = null;
    [SerializeField]
    private Light m_remoteLight = null;
    [SerializeField]
    private Material m_emissiveMaterial = null;
    [SerializeField]
    private Color m_lightColor = Color.white;
    [Space]
    [SerializeField]
    private Transform m_lookTarget = null;
    [SerializeField]
    private float m_targetLerpCoeffient = 5f;

    [SyncVar]
    private bool m_lightState = false;

    private bool m_followCamera = false;

    private void Awake ()
    {
        Debug.Assert ( m_networkIdentity != null, "" );
        Debug.Assert ( m_lightObject != null, "m_lightObject is null." );
        Debug.Assert ( m_localLight != null, "m_headlampLight is null." );
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_followCamera = m_networkIdentity.isLocalPlayer;
    }

    // Update is called once per frame
    private void Update()
    {
        // Input handling
        bool input = InputHandler.ReadButtonOnce ( this, TOGGLE_ACTION_NAME );
        if ( input && isLocalPlayer )
        {
            if ( isServer )
            {
                RpcToggleLight ( m_lightState = !m_lightState );
            }
            else
            {
                CmdToggleLight ( m_lightState );
            }
        }
    }

    private void LateUpdate ()
    {
        // Follow look target
        if ( m_followCamera )
        {
            m_lightObject.localRotation = Quaternion.Slerp ( m_lightObject.localRotation, m_lookTarget.localRotation, Time.deltaTime * m_targetLerpCoeffient );
        }
    }

    [Command]
    public void CmdToggleLight ( bool lightState )
    {
        // Toggle state flag
        m_lightState = !lightState;

        RpcToggleLight ( m_lightState );
    }

    [ClientRpc]
    public void RpcToggleLight ( bool lightState )
    {
        // Toggle state flag
        m_lightState = lightState;

        // Set Light component state
        if ( isLocalPlayer )
        {
            m_localLight.enabled = m_lightState;
        }
        else
        {
            m_remoteLight.enabled = m_lightState;
        }

        // Toggle emissive property of material
        if ( m_emissiveMaterial != null )
        {
            if ( m_lightState )
            {
                // Material emission on
                m_emissiveMaterial.EnableKeyword ( "_EMISSION" );
                m_emissiveMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
                m_emissiveMaterial.SetColor ( "_EmissionColor", m_lightColor );
            }
            else
            {
                // Material emission off
                m_emissiveMaterial.DisableKeyword ( "_EMISSION" );
                m_emissiveMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                m_emissiveMaterial.SetColor ( "_EmissionColor", Color.black );
            }
        }
    }
}
