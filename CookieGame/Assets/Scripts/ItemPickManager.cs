using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem; // �ܼ� ������
    private Vector3 offset;
    private Vector3 beforeScale;

    [SerializeField] private float pickupScale = 1.2f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zDistance = 5.0f;

    void Update()
    {
        if (Input.touchCount > 0) // ��ġ �Է��� 0 �̻��� ���-��� �ϳ� �̻��� ��ġ�� ������ ���
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {// ������Ʈ ��ŷ
                        //Ray ray = Camera.main.ScreenPointToRay(touch.position); // ray�� ����� ������Ʈ ����
                        //RaycastHit hit;

                        // 2D�� ����
                        Vector2 wp = Camera.main.ScreenToWorldPoint(touch.position);
                        Ray2D ray = new Ray2D(wp, Vector2.zero);
                        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                        // if(Physics.Raycast(ray, out hit)) ���� 2D�� ����
                        if (hit2D.collider != null)
                        {
                            //if (hit.collider != null)
                            // ������ ����ϴ� �Ϲ� raycast�� 3D�� ���� �����
                            // Ray2D�� RaycastHit2D�� ����ؾ� ��
                            foodItem = hit2D.collider.gameObject;
                            beforeScale = foodItem.transform.localScale;

                            foodItem.transform.localScale = beforeScale * pickupScale;

                            Vector3 itempos = foodItem.transform.position;
                            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                new Vector3(touch.position.x, touch.position.y, zDistance)
                            );
                            offset = itempos - touchWorldPos;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {// ������Ʈ �̵�
                        if (foodItem != null)
                        {
                            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                    new Vector3(touch.position.x, touch.position.y, zDistance)
                            );
                            Vector3 targetPos = touchWorldPos + offset;

                            foodItem.transform.position = Vector3.Lerp(
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
