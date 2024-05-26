using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private Animator animator;

    [SerializeField]
    private CameraManager cameraManager;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerLocomotion.GetState() == PlayerLocomotion.State.Idle)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
        } else if (playerLocomotion.GetState() == PlayerLocomotion.State.Walking)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
        } else
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
        }
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovements();
    }

    private void LateUpdate()
    {
        cameraManager.HandleMovement();
    }
}
