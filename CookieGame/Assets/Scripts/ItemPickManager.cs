using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem; // ������ ������ ����
    private Vector2 offset; // ������ �̵��� ����� offset
    private Vector2 beforeScale; // ������ �ʱ� ũ��

    [SerializeField] private float pickupScale = 1.2f; // ������ ����� �� Ŀ�� ����
    [SerializeField] private float moveSpeed = 10f; // �������� ��ġ�� ���󰡴� �ӵ�

    void Update()
    {
        if (Input.touchCount > 0) // ��ġ �Է��� 0 �̻��� ���-��� �ϳ� �̻��� ��ġ�� ������ ���
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {// ������Ʈ ��ŷ
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
                    {// ������Ʈ �̵�
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
                    {// ������Ʈ �ű��
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
