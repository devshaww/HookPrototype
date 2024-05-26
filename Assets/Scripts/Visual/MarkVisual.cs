
using UnityEngine;
using UnityEngine.UI;

public class MarkVisual : MonoBehaviour
{
    [SerializeField] private Image circleImage;

    private float maxScale = 1.5f;

    private void OnEnable()
    {
        circleImage.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float currentScale = circleImage.rectTransform.localScale.x;
        if (currentScale >= maxScale) return;
        float scale = Mathf.MoveTowards(currentScale, maxScale, Time.deltaTime);
        circleImage.rectTransform.localScale = new Vector3(scale, scale, 1);
    }
}
