using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
	// does not get destroyed automatically
	private PlayerInputAction playerInputAction;

	public static InputHandler Instance { get; private set; }

	public bool jumpInput { get; private set; }
	public bool sprintInput { get; private set; }
	public Vector2 lookInput { get; private set; }
	public Vector2 moveInput { get; private set; }
	public bool hookInput { get; private set; }

	private void Awake()
	{
		// won't get destroyed automatically even if InputHander has been destroyed when scene changes
		Instance = this;
		playerInputAction = new PlayerInputAction();
		playerInputAction.Player.Enable();
		playerInputAction.Player.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
		playerInputAction.Player.Look.performed += i => lookInput = i.ReadValue<Vector2>();
		playerInputAction.Player.Jump.performed += (obj) => jumpInput = true;
		playerInputAction.Player.Hook.performed += (obj) => hookInput = true;
		playerInputAction.Player.Sprint.performed += (obj) => sprintInput = true;
		playerInputAction.Player.Sprint.canceled += (obj) => sprintInput = false;
	}

    private void OnDestroy()
    {
		//playerInputAction.Player.Jump.performed -= (obj) => jumpInput = true;
  //      playerInputAction.Player.Hook.performed -= Hook_performed;
		//playerInputAction.Player.Sprint.performed -= (obj) => sprintInput = true;
		//playerInputAction.Player.Sprint.canceled -= (obj) => sprintInput = false;
		playerInputAction.Disable();
		playerInputAction.Dispose();
    }

	public void HandleAllInputs()
	{
		//HandleJumpInput();
		//HandleHookInput();
	}

	private void HandleJumpInput()
    {
  //      if (jumpInput)
		//{
		//	jumpInput = false;
		//}
    }

	public void UseHookInput()
	{
		hookInput = false;
	}

	public void UseJumpInput()
	{
		jumpInput = false;
	}
}
