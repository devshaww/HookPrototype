using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;

    [SerializeField]
    private CameraManager cameraManager;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
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
