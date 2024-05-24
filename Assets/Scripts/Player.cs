using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private HookComponent hookComponent;
    private Animator animator;

    [SerializeField]
    private CameraManager cameraManager;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        hookComponent = GetComponent<HookComponent>();
        animator = GetComponent<Animator>();
    }

    //private void Start()
    //{
    //    playerLocomotion.SetVelocityTest(new Vector3(2,13,5));
    //}

    private void Update()
    {
        InputHandler.Instance.HandleAllInputs();  // doing nothing for now
        hookComponent.HookStatusUpdate();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovements();
    }

    private void LateUpdate()
    {
        cameraManager.HandleMovement();
        hookComponent.HookStatusLateUpdate();
    }
}
