using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    private GameObject foodItem;    // ���� ��� �ִ� ������
    private Vector2 offset;         // �������� ������ ��, ��ġ ��ġ�� ������ ��ġ ������ �Ÿ�
    private Vector2 beforeScale;    // �������� ���� ũ�� (���� ��)

    [SerializeField] private float pickupScale = 1.2f; // �������� ����� �� Ŀ�� ����
    [SerializeField] private float moveSpeed = 10f;    // �������� ��ġ�� ���󰡴� �ӵ�

    // ���� �������� z���� �����ص� ����
    private float originalZ = 0f;

    void Update()
    {
        // ��ġ�� �ϳ� �̻� �����Ǹ� ����
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
                            // ��ġ�� ������Ʈ�� foodItem���� ����
                            foodItem = hit2D.collider.gameObject;

                            // �������� ���� ������(ũ��) ���� ��, ��¦ Ŀ����
                            beforeScale = foodItem.transform.localScale;
                            foodItem.transform.localScale = beforeScale * pickupScale;

                            // ������ ��ġ : z�� ����
                            Vector3 itemPos3D = foodItem.transform.position;
                            originalZ = itemPos3D.z;

                            // �����۰� ��ġ�� ���� ����/�Ÿ���ŭ ������ ������, �׸�ŭ ������Ʈ �̵��� �����ϱ� ����
                            Vector2 itemPos2D = new Vector2(itemPos3D.x, itemPos3D.y);
                            offset = itemPos2D - touchWorldPos;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (foodItem != null)
                        {
                            // ���� ��ġ ��ġ(���� ��ǥ)
                            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

                            // offset �ݿ��Ͽ�, ���� ��ǥ ��ġ�� ���
                            Vector2 targetPos2D = touchWorldPos + offset;

                            // ���� ��ġ�� ��ǥ ��ġ�� �����ͼ�, z�� originalZ ����
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
                            // ũ�⸦ ������� ����
                            foodItem.transform.localScale = beforeScale;
                        }
                        // ���� ������ ���� �ʱ�ȭ
                        foodItem = null;
                        break;
                    }
            }
        }
    }
}
