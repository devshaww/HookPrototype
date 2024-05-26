using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    private Vector3 cameraFollowVeclocity = Vector3.zero;
    private float cameraFollowSpeed = .01f;
    private float cameraLookSpeed = 2f;
    private float cameraPivotSpeed = 2f;

    private float horizontalAngle;
    private float verticalAngle;
    private float maxVerticalAngle = 120f;
    private float minVerticalAngle = -35f;

    public void HandleMovement()
    {
        FollowTarget();
        RotateCamera();
    }

    private void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVeclocity, cameraFollowSpeed);
        transform.position = targetPos;
    }

    public void RotateCamera()
    {
        Vector2 lookInput = InputHandler.Instance.lookInput;
        horizontalAngle += lookInput.x * cameraLookSpeed;
        verticalAngle -= lookInput.y * cameraPivotSpeed;   // 向y负轴方向转是正
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

        Vector3 rotation = new(verticalAngle, horizontalAngle, 0);
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //rotation = new(verticalAngle, 0, 0);
        //targetRotation = Quaternion.Euler(rotation);
        //cameraPivot.localRotation = targetRotation;
    }
}
