using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [Header("반죽 Transform")]
    [SerializeField] private Transform dough;        // 변경시킬 반죽 오브젝트  
    [SerializeField] private float scaleFactor = 0.001f; // 스케일 변경 비율

    [Header("반죽 최대 크기 / 다음단계 임계값")]
    [SerializeField] private float maxScaleX = 50f;   // x축 최대 크기  
    [SerializeField] private float maxScaleY = 50f;   // y축 최대 크기
    [SerializeField] private float thresholdScaleX = 50f; // x축 다음단계 임계값
    [SerializeField] private float thresholdScaleY = 50f; // y축 다음단계 임계값

    [Header("연관 스크립트 & 오브젝트")]
    [SerializeField] private Dough2D dough2d;
    [SerializeField] private CookieShapeManager CookieShape;
    public GameObject rollingPin;

    private Vector2 lastTouchPos;

    void Start()
    {
        // 예) 처음에는 반죽을 (5,5,1) 정도로 작게 시작시켜야
        // 롤링 동작(드래그) 중에 커지는 걸 체감할 수 있음
        dough.localScale = new Vector3(5f, 5f, 1f);
    }

    void Update()
    {
        // Rolling pin과 충돌 중이 아니면 크기를 변경하지 않음
        if (!dough2d.isCollidedPin) return;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lastTouchPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                // 이동 거리만큼 반죽을 확장
                float distance = t.deltaPosition.magnitude;

                Vector2 newScale = dough.localScale;
                newScale.x += distance * scaleFactor;
                // y는 x보다 조금 더 적게/많이 늘리려면 가중치 조절
                newScale.y += distance * scaleFactor / 0.5f;

                // 최대 크기 제한
                newScale.x = Mathf.Clamp(newScale.x, 1.3f, maxScaleX);
                newScale.y = Mathf.Clamp(newScale.y, 1.0f, maxScaleY);

                dough.localScale = newScale;

                // X, Y 둘 다 15를 넘으면 다음 단계로 (원하는 임계값 조정 가능)
                if (newScale.x >= thresholdScaleX && newScale.y >= thresholdScaleY)
                {
                    rollingPin.SetActive(false);
                    CookieShape.CookieCutterShow();
                }
            }
        }
    }
}
