using UnityEngine;

[ExecuteAlways]
public class UGUIRadialLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    public float radius = 200f;            // 반지름(px)
    [Range(0f, 360f)] public float startAngle = 90f; // 0=오른쪽, 90=위쪽
    [Range(0f, 360f)] public float spanAngle = 360f; // 전체 각도(360=풀서클)
    public bool clockwise = true;          // 시계방향 여부
    public bool faceOutward = false;       // 아이콘이 바깥쪽을 보게 회전
    public float rotationOffset = 0f;      // 회전 보정(도)

    void OnEnable() => Arrange();
    void OnValidate() => Arrange();

    public void Arrange()
    {
        // 자식 RectTransform들만 수집
        var children = new System.Collections.Generic.List<RectTransform>();
        foreach (Transform t in transform)
        {
            if (t.gameObject.activeInHierarchy)
            {
                RectTransform rt = t as RectTransform;
                if (rt != null) children.Add(rt);
            }
        }

        int n = children.Count;
        if (n == 0) return;

        bool fullCircle = Mathf.Approximately(spanAngle, 360f);
        float step = (fullCircle ? spanAngle / n : (n == 1 ? 0f : spanAngle / (n - 1)));

        for (int i = 0; i < n; i++)
        {
            RectTransform child = children[i];
            child.anchorMin = child.anchorMax = new Vector2(0.5f, 0.5f);
            child.pivot = new Vector2(0.5f, 0.5f);

            float angle = startAngle + (clockwise ? -step * i : step * i);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            child.anchoredPosition = pos;

          
            float z = 0f;

            // 방향 옵션에 따라 회전값 계산
            if (faceOutward) z = angle - 90f;        // 바깥쪽을 향함
            else z = angle + 90f;                    // 중심을 향함

            child.localRotation = Quaternion.Euler(0, 0, z + rotationOffset);
           
        }
    }
}
