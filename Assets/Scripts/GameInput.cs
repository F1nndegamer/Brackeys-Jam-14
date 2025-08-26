using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    public event Action OnInteract;
    public event Action OnSmokeBomb;

    private InputSystem_Actions inputSystem;

    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Player.Enable();

        inputSystem.Player.Interact.performed += Interact_performed;
        inputSystem.Player.SmokeBomb.performed += SmokeBomb_performed; ;
    }

    private void SmokeBomb_performed(InputAction.CallbackContext obj)
    {
        OnSmokeBomb?.Invoke();
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke();
    }

    public Vector2 GetMovementVector()
    {
        return inputSystem.Player.Move.ReadValue<Vector2>();
    }

    public bool GetRunHeld()
    {
        return inputSystem.Player.Sprint.IsPressed();
    }
    public bool GetCrouchHeld()
    {
        return inputSystem.Player.Crouch.IsPressed();
    }
    private void OnDestroy()
    {
        inputSystem.Player.Interact.performed -= Interact_performed;
        inputSystem.Dispose();
    }
}
