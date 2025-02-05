using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem;    // 현재 잡고 있는 아이템
    private Vector2 offset;         // 아이템을 집었을 때, 터치 위치와 아이템 위치 사이의 거리
    private Vector2 beforeScale;    // 아이템의 원래 크기 (집기 전)

    [SerializeField] private float pickupScale = 1.2f; // 아이템을 잡았을 때 커질 비율
    [SerializeField] private float moveSpeed = 10f;    // 아이템이 터치를 따라가는 속도

    // 원래 아이템의 z값을 저장해둘 변수
    private float originalZ = 0f;

    void Update()
    {
        // 터치가 하나 이상 감지되면 진행
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
                        Ray2D ray = new Ray2D(touchWorldPos, Vector2.zero);
                        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                        if (hit2D.collider != null)
                        {
                            // 터치된 오브젝트를 foodItem으로 지정
                            foodItem = hit2D.collider.gameObject;

                            // 아이템의 원래 스케일(크기) 저장 후, 살짝 커지게
                            beforeScale = foodItem.transform.localScale;
                            foodItem.transform.localScale = beforeScale * pickupScale;

                            // 아이템 위치 : z값 저장
                            Vector3 itemPos3D = foodItem.transform.position;
                            originalZ = itemPos3D.z;

                            // 아이템과 터치가 일정 각도/거리만큼 떨어져 있으면, 그만큼 오브젝트 이동에 보정하기 위해
                            Vector2 itemPos2D = new Vector2(itemPos3D.x, itemPos3D.y);
                            offset = itemPos2D - touchWorldPos;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (foodItem != null)
                        {
                            // 현재 터치 위치(월드 좌표)
                            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

                            // offset 반영하여, 실제 목표 위치를 계산
                            Vector2 targetPos2D = touchWorldPos + offset;

                            // 기존 위치와 목표 위치를 가져와서, z만 originalZ 유지
                            Vector3 currentPos3D = foodItem.transform.position;
                            Vector3 targetPos3D = new Vector3(targetPos2D.x, targetPos2D.y, originalZ);

                            foodItem.transform.position = Vector3.Lerp(
                                currentPos3D,
                                targetPos3D,
                                Time.deltaTime * moveSpeed
                            );
                        }
                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        if (foodItem != null)
                        {
                            // 크기를 원래대로 복귀
                            foodItem.transform.localScale = beforeScale;
                        }
                        // 현재 아이템 정보 초기화
                        foodItem = null;
                        break;
                    }
            }
        }
    }
}
