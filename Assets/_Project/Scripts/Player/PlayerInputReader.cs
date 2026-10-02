using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private Vector3Variable _inputMoveVariable;
    
    private PlayerControls _controller;

    private void Awake()
    {
        _controller = new PlayerControls();

        _controller.Keyboard.Move.performed += OnMovePerformed;

        _controller.Keyboard.Move.canceled += OnMoveCanceled;
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _inputMoveVariable.value = context.ReadValue<Vector3>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _inputMoveVariable.value = Vector3.zero;
    }

    private void OnEnable()
    {
        _controller.Enable();
    }

    private void OnDisable()
    {
        _controller.Disable();
    }

}
