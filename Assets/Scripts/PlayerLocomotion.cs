using System.Collections;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private int moveSpeed;
    [SerializeField] private int sprintSpeed;
    [SerializeField] private int rotationSpeed;

    private CollisionSense collisionSense;
    private HookComponent hookComponent;
    private Rigidbody rb;
    private float inAirTimer;

    public bool isSprinting;
    public bool isWalking;
    public bool isFreezing;
    //public bool isJumping;
    public float jumpHeight = 3f;
    public float gravity = -15f;

    private Vector3 velocityToSetUsingHook;
	private Vector3 moveDirection;

	Transform mainCamera;

    private void Awake()
    {
        hookComponent = GetComponent<HookComponent>();
        collisionSense = GetComponent<CollisionSense>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main.transform;
    }

    public void SetFreeze(bool freeze)
    {
        if (freeze)
        {
            isFreezing = true;
            rb.velocity = Vector3.zero;
        }
        else
        {
            isFreezing = false;
        }
    }

    public void HandleAllMovements()
    {
        // 第二个费时间找问题的点：斜抛运动老是跳不到指定点，是因为忘了确认isDuringHook导致CheckFall被调用影响速度。
        if (isFreezing || hookComponent.isDuringHook) return;
        CheckMove();
        CheckJump();
        CheckFall();
    }

    public void CheckMove()
    {
        if (collisionSense.Grounded)
        {
            Vector2 movementInput = InputHandler.Instance.moveInput;
            // 设置Player向抓钩点的速度后一直静止的原因...没有check输入是否为0向量，导致设置速度后立马被设置成0，找了好久。
            isSprinting = InputHandler.Instance.sprintInput;
            moveDirection = mainCamera.forward * movementInput.y + mainCamera.right * movementInput.x;
            moveDirection.Normalize();
            moveDirection.y = 0;

            isWalking = moveDirection != Vector3.zero;
 
            rb.velocity = isSprinting ? moveDirection * sprintSpeed : moveDirection * moveSpeed;

            //Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            //Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //transform.rotation = playerRotation;
        }
        else
        {
            isWalking = false;
            isSprinting = false;
        }
    }

    public void CheckJump()
    {
        if (collisionSense.Grounded)
        {
            if (InputHandler.Instance.jumpInput)
            {
                float velocityY = Mathf.Sqrt(-2 * jumpHeight * gravity);
                rb.velocity = new Vector3(rb.velocity.x, velocityY, rb.velocity.z);
                //InputHandler.Instance.UseJumpInput();
            }
        }
    }

    public void CheckFall()
    {
        if (!collisionSense.Grounded)
        {
            inAirTimer += Time.deltaTime;
            rb.AddForce(3 * gravity * inAirTimer * Vector3.up);
        } else
        {
            //isJumping = false;
            inAirTimer = 0;
        }
    }

    private void SetVelocity()
    {
        rb.velocity = velocityToSetUsingHook;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryMaxHeight)
    {
        velocityToSetUsingHook = CalculateJumpVelocity(transform.position, targetPosition, trajectoryMaxHeight);
        Invoke(nameof(SetVelocity), .1f);
    }

    private Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float trajectoryMaxHeight)
    {
        float gravity = Physics.gravity.y;
        float distanceY = end.y - start.y;
        Vector3 distanceXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryMaxHeight);
        Vector3 velocityXZ = distanceXZ / (Mathf.Sqrt(-2 * trajectoryMaxHeight / gravity) + Mathf.Sqrt(2 * (distanceY - trajectoryMaxHeight) / gravity));
        return velocityXZ + velocityY;
    }
}
