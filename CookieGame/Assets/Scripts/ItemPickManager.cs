using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem; // 아이템 저장할 변수
    private Vector2 offset; // 아이템 이동에 사용할 offset
    private Vector2 beforeScale; // 아이템 초기 크기

    [SerializeField] private float pickupScale = 1.2f; // 아이템 잡았을 때 커질 비율
    [SerializeField] private float moveSpeed = 10f; // 아이템이 터치를 따라가는 속도

    void Update()
    {
        if (Input.touchCount > 0) // 터치 입력이 0 이상인 경우-적어도 하나 이상의 터치가 감지된 경우
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {// 오브젝트 픽킹
                        Vector2 wp = Camera.main.ScreenToWorldPoint(touch.position);
                        Ray2D ray = new Ray2D(wp, Vector2.zero);
                        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                        if (hit2D.collider != null)
                        {
                            foodItem = hit2D.collider.gameObject;
                            beforeScale = foodItem.transform.localScale;

                            foodItem.transform.localScale = beforeScale * pickupScale;

                            Vector2 itempos = foodItem.transform.position;
                            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                new Vector2(touch.position.x, touch.position.y)
                            );
                            offset = itempos - touchWorldPos;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {// 오브젝트 이동
                        if (foodItem != null)
                        {
                            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                    new Vector2(touch.position.x, touch.position.y)
                            );
                            Vector2 targetPos = touchWorldPos + offset;

                            foodItem.transform.position = Vector2.Lerp(
                                foodItem.transform.position,
                                targetPos,
                                Time.deltaTime * moveSpeed
                            );
                        }
                        break;
                    }

                case TouchPhase.Ended:
                    {// 오브젝트 옮기기
                        if (foodItem != null)
                        {
                            foodItem.transform.localScale = beforeScale;
                        }
                        foodItem = null;
                        break;
                    }
            }
        }
    }
}
