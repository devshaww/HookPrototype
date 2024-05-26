
using UnityEngine;

public class HookableVisual : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject markPrefab;

    private GameObject markTarget;
    private GameObject indicatorTarget;
    private float offsetLeft = .1f * Screen.width;
    private float offsetRight = .1f * Screen.width;
    private float offsetUp = .1f * Screen.height;
    private float offsetDown = .1f * Screen.height;

    // Start is called before the first frame update
    void Start()
    {
        indicatorPrefab.SetActive(false);
        markPrefab.SetActive(false);
    }

    public void SetIndicatorTarget(GameObject t)
    {
        if (t == indicatorTarget) return;
        indicatorTarget = t;
    }

    public void SetMarkTarget(GameObject t)
    {
        if (t == markTarget) return;
        markTarget = t;
    }

    public void ClearMarkTarget()
    {
        markTarget = null;
        markPrefab.SetActive(false);
    }

    public void ClearIndicatorTarget()
    {
        indicatorTarget = null;
        indicatorPrefab.SetActive(false);
    }

    public void ClearAllTargets()
    {
        ClearMarkTarget();
        ClearIndicatorTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (markTarget != null)
        {
            UpdateMark();
        }
        else if (indicatorTarget != null)
        {
            UpdateIndicator();
        }
    }

    private void UpdateMark()
    {
        markPrefab.transform.position = Camera.main.WorldToScreenPoint(markTarget.transform.position);
        markPrefab.SetActive(true);
    }

    private Vector3 IndicatorClamp(Vector3 newPos)
    {
        Vector2 center = new(Screen.width / 2, Screen.height / 2);
        float k = (newPos.y - center.y) / (newPos.x - center.x);

        if (newPos.y - center.y > 0)
        {
            newPos.y = Screen.height - offsetUp;
            newPos.x = center.x + (newPos.y - center.y) / k;
        }
        else
        {
            newPos.y = offsetDown;
            newPos.x = center.x + (newPos.y - center.y) / k;
        }

        if (newPos.x > Screen.width - offsetRight)
        {
            newPos.x = Screen.width - offsetRight;
            newPos.y = center.y + (newPos.x - center.x) * k;
        }
        else if (newPos.x < offsetLeft)
        {
            newPos.x = offsetLeft;
            newPos.y = center.y + (newPos.x - center.x) * k;
        }

        return newPos;
    }

    private void UpdateIndicator()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(indicatorTarget.transform.position);
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            screenPos = IndicatorClamp(screenPos);
            indicatorPrefab.transform.position = screenPos;

            Vector3 center = new(Screen.width / 2, Screen.height / 2, 0);
            Vector2 direction = screenPos - center;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 第三个坑点：旋转
            // 1.在编辑器里设置的Image的rectTranform.rotation并不是localRotation 并没有改变其初始状态 在代码里设置transform.rotation并不是以rectTranform.rotation为原始状态来旋转 而是相同的东西
            // 2.Quaternion.Euler基于旋转度数决定是顺时针还是逆时针 传入的度数d会被限制在(-180,180) d大于180则变成-(360-d) 
            // 3.angle为-180~180

            indicatorPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+90));
            indicatorPrefab.SetActive(true);
        }
    }
}
