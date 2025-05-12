using UnityEngine;

public class PlayerInputHandler : MonoBehaviour, PlayerInput.IGameplayActionsActions
{
    private PlayerInput input;

    private Cached<PlayerShooting> cached_Shooting;
    private PlayerShooting Shooting => cached_Shooting[this];

    private Cached<Dash> cached_Dash;
    private Dash Dash => cached_Dash[this];

    private Cached<Movement> cached_Movement;
    private Movement Movement => cached_Movement[this];


    public void OnAim(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Shooting.Direction = context.ReadValue<Vector2>();
    }

    public void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Dash.ButtonPressed = context.ReadValueAsButton();
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Movement.Direction = context.ReadValue<Vector2>();
    }

    public void OnShoot(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Shooting.ButtonPressed = context.ReadValueAsButton();
    }

    void Awake()
    {
        input = new();
        input.GameplayActions.SetCallbacks(this);
        input.GameplayActions.Enable();
    }
}
