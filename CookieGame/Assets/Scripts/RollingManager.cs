using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [Header("반죽 Transform")]
    [SerializeField] private Transform dough;
    [SerializeField] private float scaleFactor = 0.001f;

    [Header("반죽 최대 크기 / 다음단계 임계값")]
    [SerializeField] private float maxScaleX = 35f;
    [SerializeField] private float maxScaleY = 20f;
    [SerializeField] private float thresholdScaleX = 35f;
    [SerializeField] private float thresholdScaleY = 20f;

    [Header("연관 스크립트 & 오브젝트")]
    [SerializeField] private Dough2D dough2d;
    [SerializeField] private CookieShapeManager CookieShape;
    public GameObject rollingPin;

    private Vector2 initialScale;

    private float totalDragDistance = 0f;

    void Start()
    {
        initialScale = dough.localScale;
    }

    void Update()
    {
        if (!dough2d.isCollidedPin) return;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                // 매 프레임 이동 거리를 누적
                float distance = t.deltaPosition.magnitude;
                totalDragDistance += distance;

                // 누적 거리를 통해 스케일을 계산
                float newX = initialScale.x + totalDragDistance * scaleFactor;
                float newY = initialScale.y + totalDragDistance * (scaleFactor / 0.5f);

                // 최대 크기 제한
                newX = Mathf.Clamp(newX, 1.3f, maxScaleX);
                newY = Mathf.Clamp(newY, 1.0f, maxScaleY);

                dough.localScale = new Vector2(newX, newY);

                // 임계값 체크
                if (newX >= thresholdScaleX && newY >= thresholdScaleY)
                {
                    rollingPin.SetActive(false);
                    CookieShape.CookieCutterShow();
                }
            }
        }
    }
}
