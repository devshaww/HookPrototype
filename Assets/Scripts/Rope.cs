using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private HookComponent hookComponent;
    private Vector2 currentEnd;
    private Spring spring;

    public int quality = 200;
    public float damper = 14;
    public float strength = 800;
    public float velocity = 15;
    public float waveCount = 3;
    public float waveHeight = 1;
    public AnimationCurve affectCurve;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    // 绳子伸长的过程
    private void DrawRope()
    {
        if (!hookComponent.isDuringHook)
        {
            currentEnd = hookComponent.GetStartingPoint().position;
            //spring.Reset();
            if (lr.positionCount > 0)
            {
                lr.positionCount = 0;
            }
            return;
        }

        if (lr.positionCount == 0)
        {
            //spring.SetVelocity(velocity);
            //lr.positionCount = quality + 1;
            lr.positionCount = 2;
        }
        //spring.SetDamper(damper);
        //spring.SetStrength(strength);
        //spring.Update(Time.deltaTime);

        Vector3 hitPoint = hookComponent.GetHitPoint();
        Vector3 startingPoint = hookComponent.GetStartingPoint().position;
        //Vector3 up = Quaternion.LookRotation((hitPoint - startingPoint).normalized) * Vector3.up;

        currentEnd = Vector3.Lerp(currentEnd, hitPoint, Time.deltaTime);

        //for (int i = 0; i < quality + 1; i++)
        //{
        //    float delta = i / (float)quality;
        //    Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
        //    lr.SetPosition(i, Vector3.Lerp(startingPoint, currentEnd, delta) + offset);
        //}
        lr.SetPosition(0, startingPoint);
        lr.SetPosition(1, currentEnd);
    }

}
