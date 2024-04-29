using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerInputHandler : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    public Vector2 MovementVector;

    private void Awake()
    {
        if (isLocalPlayer)
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
            
        }
    }
    public void Movement_Performed(InputAction.CallbackContext ctx)
    {
        MovementVector = ctx.ReadValue<Vector2>();
    }
    public void Jump_Performed(InputAction.CallbackContext ctx)
    {

    }
}
