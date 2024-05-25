using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookComponent : MonoBehaviour
{
    [SerializeField] private float overshootY;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask whatIsHookable;
    [SerializeField] private Transform startingPoint;  // 甩出绳索的起点
    [SerializeField] private HookableVisual visual;

    public float maxReachingDistance;   // 绳索能到的最远距离
    public float delayTime = 1;    // 直到玩家被拉走的时间间隔

    private Vector3 currentEnd;
    private float thresholdAngle = 60;
    private int detectRange = 40;
    private PlayerLocomotion playerLocomotion;
    private LineRenderer lr;
    //private float hookCooldown = 1;
    //private float hookCooldownTimer;
    private Vector3 hitPoint;   // 勾中目标位置
    private SpringJoint joint;
    private GameObject prevDetectedObject;

    GameObject hookableToJumpTo = null;
    public bool isDuringHook;  // 是否处于钩锁状态

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        lr = startingPoint.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        CheckHookable();
        if (!InputHandler.Instance.hookInput || hookableToJumpTo == null) return;
        Throw();
    }

    private void LateUpdate()
    {
        //DrawRope();
        if (!isDuringHook) return;
        lr.SetPosition(0, startingPoint.position);
    }

    public Vector3 GetHitPoint()
    {
        return hitPoint;
    }

    public Transform GetStartingPoint()
    {
        return startingPoint;
    }

    public void CheckHookable()
    {
        GameObject objectToSelect = null;

        Collider[] detectResults = Physics.OverlapSphere(transform.position, detectRange, whatIsHookable);
        float minAngle = Mathf.Infinity;
        foreach (Collider collider in detectResults)
        {
            Vector3 directionToHitColliderFromCamera = collider.transform.position - cam.position;
            float angle = Vector3.Angle(directionToHitColliderFromCamera, cam.forward);
            if (angle < thresholdAngle && angle < minAngle)
            {
                minAngle = angle;
                objectToSelect = collider.gameObject;
            }
        }

        if (objectToSelect == null)
        {
            hookableToJumpTo = null;
            visual.ClearAllTargets();
            return;
        }

        Vector3 objectPos = Camera.main.WorldToViewportPoint(objectToSelect.transform.position);
        if (objectPos.x > 1 || objectPos.x < 0 || objectPos.y > 1 || objectPos.y < 0)
        {
            // indicator
            visual.SetIndicatorTarget(objectToSelect);
            visual.ClearMarkTarget();
            //Debug.Log("indicator");
            //Debug.Log(objectToSelect.name);
        } else
        {
            // mark
            hookableToJumpTo = objectToSelect;
            hitPoint = objectToSelect.transform.position;
            visual.SetMarkTarget(objectToSelect);
            visual.ClearIndicatorTarget();
            //Debug.Log("mark");
            //Debug.Log(objectToSelect.name);
        }
    }

    IEnumerator ShootLine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / delayTime;

            Vector3 currentPos = Vector3.Lerp(startingPoint.position, hitPoint, t);
            lr.SetPosition(1, currentPos);

            yield return null;
        }
        lr.SetPosition(1, hitPoint);
    }

    public void Throw()
    {
        if (hookableToJumpTo == null) return;
        isDuringHook = true;
        playerLocomotion.SetFreeze(true);
        lr.enabled = true;
        StartCoroutine(ShootLine());
        Invoke(nameof(Process), delayTime);
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
        lr.enabled = false;
        //InputHandler.Instance.UseHookInput();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(cam.position, cam.position + cam.forward * maxReachingDistance);
    //}
}
