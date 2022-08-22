using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderWire.Input;
using Mirror;

public class HeadlampController : NetworkBehaviour
{
    private const string TOGGLE_ACTION_NAME = "ToggleFlashlight";

    [Header("References")]
    [SerializeField] private Transform m_lightObject = null;
    [SerializeField] private GameObject m_localLight = null;
    [SerializeField] private GameObject m_remoteLight = null;
    [SerializeField] private Renderer m_remoteLightRenderer = null;
    [SerializeField] private Material m_emissiveMaterial = null;
    [SerializeField] private Material m_dullMaterial = null;
    [Space]
    [SerializeField] private Transform m_lookTarget = null;
    [SerializeField, Range(0.01f, 20f)] private float m_rotateSpeed = 5f;

    [SyncVar] private bool m_lightState = false;
    private bool m_followCamera = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_followCamera = isLocalPlayer;
        m_remoteLight.SetActive(m_lightState);
    }

    // Update is called once per frame
    private void Update()
    {
        // Input handling
        bool input = InputHandler.ReadButtonOnce(this, TOGGLE_ACTION_NAME);
        if (input && isLocalPlayer)
        {
            if (isServer)
            {
                RpcToggleLight(m_lightState = !m_lightState);
            }
            else
            {
                CmdToggleLight(m_lightState);
            }
        }
    }

    private void LateUpdate()
    {
        // Follow look target
        if (m_followCamera)
        {
            float diff = Vector3.Angle(m_lightObject.eulerAngles, m_lookTarget.eulerAngles);
            m_lightObject.localRotation = Quaternion.RotateTowards(
                m_lightObject.localRotation,
                m_lookTarget.localRotation,
                Time.deltaTime / diff * m_rotateSpeed);

            //float localDiff = Vector3.Angle(m_localLight.transform.eulerAngles, m_lookTarget.eulerAngles);
            //m_localLight.transform.localRotation = Quaternion.RotateTowards(
            //    m_localLight.transform.localRotation,
            //    m_lookTarget.localRotation,
            //    Time.deltaTime / localDiff * m_rotateSpeed);
        }
    }

    [Command]
    public void CmdToggleLight(bool lightState)
    {
        // Toggle state flag
        m_lightState = !lightState;

        RpcToggleLight(m_lightState);
    }

    [ClientRpc]
    public void RpcToggleLight(bool lightState)
    {
        // Toggle state flag
        m_lightState = lightState;

        // Set light object state
        if (isLocalPlayer)
        {
            m_localLight.SetActive(m_lightState);
        }
        else
        {
            m_remoteLight.SetActive(m_lightState);
        }

        // Toggle emissive property of material
        if (m_lightState)
        {
            // Material emission on
            m_remoteLightRenderer.material = m_emissiveMaterial;
            //m_emissiveMaterial.EnableKeyword ( "_EMISSION" );
            //m_emissiveMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
            //m_emissiveMaterial.SetColor ( "_EmissionColor", m_lightColor );
        }
        else
        {
            // Material emission off
            m_remoteLightRenderer.material = m_dullMaterial;
            //m_emissiveMaterial.DisableKeyword ( "_EMISSION" );
            //m_emissiveMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            //m_emissiveMaterial.SetColor ( "_EmissionColor", Color.black );
        }
    }
}
