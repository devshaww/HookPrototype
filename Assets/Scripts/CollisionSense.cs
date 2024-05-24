using UnityEngine;

public class CollisionSense : MonoBehaviour
{
    [SerializeField]
    private float groundCheckRadius, wallCheckDistance;

    [SerializeField]
    private LayerMask whatIsGround, whatIsWall;

    public bool isGrounded;

    public bool Grounded
    {
        get => isGrounded;
    }

    private void Update()
    {
        CheckGrounded();
    }

    public void CheckGrounded()
    {
        if (Physics.Linecast(transform.position, transform.position - new Vector3(0, groundCheckRadius, 0), whatIsGround))
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, groundCheckRadius, 0));
    }
}
