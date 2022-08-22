using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField, Range(0.01f, 20f)] private float m_remoteRotateSpeed = 5f;
    [SerializeField, Range(0.001f, 10f)] private float m_localRotateSpeed = 1f;
    [SerializeField] private float m_drag = 1f;
    [SerializeField] private float m_dragThreshold = 10f;

    [SyncVar] private bool m_lightState = false;
    private bool m_followCamera = false;
    private InputAction m_inputAction = null;
    private Quaternion localRotation;

    // Start is called before the first frame update
    private void Start()
    {
        m_followCamera = isLocalPlayer;
        m_remoteLight.SetActive(m_lightState);
        if (isLocalPlayer)
        {
            Debug.Log("m_inputAction = new InputAction");
            m_inputAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta");
            m_inputAction.Enable();
        }
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
        if (m_followCamera)
        {
            // Follow look target
            float diff = Vector3.Angle(m_lightObject.eulerAngles, m_lookTarget.eulerAngles);
            m_lightObject.localRotation = Quaternion.RotateTowards(
                m_lightObject.localRotation,
                m_lookTarget.localRotation,
                Time.deltaTime / diff * m_remoteRotateSpeed);

            UpdateFlashlight();
        }
    }
    private void UpdateFlashlight()
    {
        Vector2 delta = m_inputAction.ReadValue<Vector2>();
        float y = -(delta.x) * m_drag;
        float x = (delta.y) * m_drag;
        if (m_drag >= 0) // light lags behind camera
        {
            y = (y > m_dragThreshold) ? m_dragThreshold : y;
            y = (y < -m_dragThreshold) ? -m_dragThreshold : y;
            x = (x > m_dragThreshold) ? m_dragThreshold : x;
            x = (x < -m_dragThreshold) ? -m_dragThreshold : x;
        }
        else // camera lags behind light
        {
            y = (y < m_dragThreshold) ? m_dragThreshold : y;
            y = (y > -m_dragThreshold) ? -m_dragThreshold : y;

            x = (x < m_dragThreshold) ? m_dragThreshold : x;
            x = (x > -m_dragThreshold) ? -m_dragThreshold : x;
        }
        float fDiff = Vector3.Angle(localRotation.eulerAngles, m_localLight.transform.localEulerAngles);
        Quaternion newRotation = Quaternion.Euler(localRotation.x + x, localRotation.y + y, localRotation.z);
        m_localLight.transform.localRotation = Quaternion.Lerp(
            m_localLight.transform.localRotation,
            newRotation,
            Time.deltaTime * m_localRotateSpeed);
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
        }
        else
        {
            // Material emission off
            m_remoteLightRenderer.material = m_dullMaterial;
        }
    }
}
