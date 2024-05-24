using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookComponent : MonoBehaviour
{
    [SerializeField] private float overshootY;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask whatIsHookable;
    [SerializeField] private Transform startingPoint;  // 甩出绳索的起点

    public float maxReachingDistance;   // 绳索能到的最远距离
    public float delayTime;    // 直到玩家被拉走的时间间隔

    private int detectRange = 40;
    private PlayerLocomotion playerLocomotion;
    private LineRenderer lr;
    //private float hookCooldown = 1;
    //private float hookCooldownTimer;
    private Vector3 hitPoint;   // 勾中目标位置
    private SpringJoint joint;

    public bool isDuringHook;  // 是否处于钩锁状态

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        lr = startingPoint.GetComponent<LineRenderer>();
    }

    public Vector3 GetHitPoint()
    {
        return hitPoint;
    }

    public Transform GetStartingPoint()
    {
        return startingPoint;
    }

    public void HookStatusUpdate()
    {
        // 只按一个键的场景下直接用在Update里用KeyDown比InputHandler好用多了...但是不想混用input system
        if (InputHandler.Instance.hookInput) {
            Throw();
            InputHandler.Instance.UseHookInput();
        }
        //if (hookCooldownTimer > 0)
        //{
        //    hookCooldownTimer -= Time.deltaTime;
        //}
        GameObject objectToSelect = null;

        Collider[] detectResults = Physics.OverlapSphere(transform.position, detectRange, whatIsHookable);
        float minAngle = Mathf.Infinity;
        foreach (Collider collider in detectResults)
        {
            Vector3 directionToHitColliderFromCamera = collider.transform.position - cam.position;
            float angleToCameraForward = Vector3.Angle(cam.forward, directionToHitColliderFromCamera);
            if (angleToCameraForward < minAngle)
            {
                minAngle = angleToCameraForward;
                objectToSelect = collider.gameObject;
            }
        }
        if (objectToSelect == null) return;
        if (minAngle > cam.GetComponent<Camera>().fieldOfView / 2)
        {
            // indicator
            //Debug.Log("indicator");
            //Debug.Log(objectToSelect.name);
        } else
        {
            // mark
            //Debug.Log("mark");
            //Debug.Log(objectToSelect.name);
        }
    }

    private void SelectHitPoint(GameObject gameObject)
    {
        
    }

    public void HookStatusLateUpdate()
    {
        if (!isDuringHook) return;
        lr.SetPosition(0, startingPoint.position);
    }

    public void Throw()
    {
        //if (hookCooldownTimer > 0) { return; }
        playerLocomotion.SetFreeze(true);
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxReachingDistance, whatIsHookable))
        {
            hitPoint = hit.point;

            //joint = startingPoint.gameObject.AddComponent<SpringJoint>();
            //joint.autoConfigureConnectedAnchor = false;
            //joint.connectedAnchor = hitPoint;

            //float distanceFromPoint = Vector3.Distance(startingPoint.position, hitPoint);

            //joint.maxDistance = distanceFromPoint * 0.8f;
            //joint.minDistance = distanceFromPoint * 0.25f;

            //joint.spring = 4.5f;
            //joint.damper = 7f;
            //joint.massScale = 4.5f;

            isDuringHook = true;
            Invoke(nameof(Process), delayTime);
        }
        else
        {
            hitPoint = cam.position + cam.forward * maxReachingDistance;
            Invoke(nameof(Stop), delayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1, hitPoint);
    }

    public void Process()
    {
        playerLocomotion.SetFreeze(false);
        Vector3 startPoint = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        float hitPointToStartPointYOffset = hitPoint.y - startPoint.y;
        float highestPointOnTrajectory = hitPointToStartPointYOffset + overshootY;
        if (hitPointToStartPointYOffset < 0)
        {
            highestPointOnTrajectory = overshootY;
        }
        playerLocomotion.JumpToPosition(hitPoint, highestPointOnTrajectory);
        Invoke(nameof(Stop), 2);
    }

    public void Stop()
    {
        playerLocomotion.SetFreeze(false);
        isDuringHook = false;
        //hookCooldownTimer = hookCooldown;
        lr.enabled = false;
        //Destroy(joint);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(cam.position, cam.position + cam.forward * maxReachingDistance);
    //}
}
