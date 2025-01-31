using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [SerializeField] private Transform dough; // 변경시킬 반죽 오브젝트
    [SerializeField] private float scaleFactor = 0.001f; // 스케일 변경 비율\
    [SerializeField] public Dough2D dough2d;
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼
    public GameObject rollingPin;
    public GameObject shape;

    private Vector2 lastTouchPos; // 최종 pos

    void Update()
    {
        if (dough2d.isCollidedPin) // Rolling pin과 Dough가 닿아(충돌)있는 경우
        {
            if (Input.touchCount > 0) // 터치 입력이 있는 경우
            {
                Touch t = Input.GetTouch(0); // 0번 터치 입력 t ( 첫 번째 터치 입력 )

                if (t.phase == TouchPhase.Began) // 터치가 시작된 경우
                {
                    lastTouchPos = t.position; // 처음 터치 입력 pos 저장
                }
                else if (t.phase == TouchPhase.Moved) // 드래그 중인 경우
                {
                    float distance = t.deltaPosition.magnitude;

                    Vector2 newScale = dough.localScale; // dought의 기존 크키를 newScale로 저장 (벡터)
                    newScale.x += distance * scaleFactor; // 이동거리 * scaleFactor 만큼 크기 변화 (x축)
                    newScale.y += distance * scaleFactor / 0.5f; // 이동거리 * scaleFactor / 0.5f 만큼 크기 변화 (y축)

                    newScale.x = Mathf.Clamp(newScale.x, 1.3f, 5f);
                    newScale.y = Mathf.Clamp(newScale.y, 1.0f, 3.7f);

                    dough.localScale = newScale;

                    if (Mathf.Approximately(newScale.x, 5f)
                        && Mathf.Approximately(newScale.y, 3.7f))
                    {
                        rollingPin.SetActive(false);
                        shape.SetActive(true);
                    }
                }
            }
        }
    }
}