using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem; // 단순 아이템
    private Vector2 offset;
    private Vector2 beforeScale;

    [SerializeField] private float pickupScale = 1.2f;
    [SerializeField] private float moveSpeed = 10f;

    void Update()
    {
        if (Input.touchCount > 0) // 터치 입력이 0 이상인 경우-적어도 하나 이상의 터치가 감지된 경우
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {// 오브젝트 픽킹
                        //Ray ray = Camera.main.ScreenPointToRay(touch.position); // ray를 사용해 오브젝트 감지
                        //RaycastHit hit;

                        // 2D로 변경
                        Vector2 wp = Camera.main.ScreenToWorldPoint(touch.position);
                        Ray2D ray = new Ray2D(wp, Vector2.zero);
                        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                        // if(Physics.Raycast(ray, out hit)) 에서 2D로 변경
                        if (hit2D.collider != null)
                        {
                            //if (hit.collider != null)
                            // 기존에 사용하던 일반 raycast는 3D일 때만 적용됨
                            // Ray2D와 RaycastHit2D를 사용해야 함
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