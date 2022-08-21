using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderWire.Input;
using Mirror;

public class ToggleObject : NetworkBehaviour
{
    [SerializeField] private string inputActionName = "ToggleTestPlayerAudio";
    [SerializeField] private List<GameObject> targets = new List<GameObject>();

    private void Start()
    {
        if (!isLocalPlayer) enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (InputHandler.ReadButtonOnce(this, inputActionName))
        {
            CmdToggleObjects();
        }
    }

    [Command]
    private void CmdToggleObjects()
    {
        targets.ForEach((GameObject o) =>
        {
            o.SetActive(!o.activeSelf);
        });
        RpcToggleObjects();
    }

    [ClientRpc]
    private void RpcToggleObjects()
    {
        if (!isServer)
        {
            targets.ForEach((GameObject o) =>
            {
                o.SetActive(!o.activeSelf);
            });
        }
    }
}
