using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(PlayerInput))]
public class PlayerControllerTest : NetworkBehaviour
{
    public InputAction moveAction;
    public float moveSpeed = 5.0f;
    public Vector2 position;

    void Start ()
    {
        if ( !isLocalPlayer )
        {
            enabled = false;
        }
        moveAction.Enable ();
    }

    void Update ()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2> ();
        transform.Translate ( moveDirection * moveSpeed * Time.deltaTime );
    }
}
