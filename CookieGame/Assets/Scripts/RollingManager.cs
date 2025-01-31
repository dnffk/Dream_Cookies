using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [SerializeField] private Transform dough; // �����ų ���� ������Ʈ
    [SerializeField] private float scaleFactor = 0.001f; // ������ ���� ����\
    [SerializeField] public Dough2D dough2d;
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư
    public GameObject rollingPin;
    public GameObject shape;

    private Vector2 lastTouchPos; // ���� pos

    void Update()
    {
        if (dough2d.isCollidedPin) // Rolling pin�� Dough�� ���(�浹)�ִ� ���
        {
            if (Input.touchCount > 0) // ��ġ �Է��� �ִ� ���
            {
                Touch t = Input.GetTouch(0); // 0�� ��ġ �Է� t ( ù ��° ��ġ �Է� )

                if (t.phase == TouchPhase.Began) // ��ġ�� ���۵� ���
                {
                    lastTouchPos = t.position; // ó�� ��ġ �Է� pos ����
                }
                else if (t.phase == TouchPhase.Moved) // �巡�� ���� ���
                {
                    float distance = t.deltaPosition.magnitude;

                    Vector2 newScale = dough.localScale; // dought�� ���� ũŰ�� newScale�� ���� (����)
                    newScale.x += distance * scaleFactor; // �̵��Ÿ� * scaleFactor ��ŭ ũ�� ��ȭ (x��)
                    newScale.y += distance * scaleFactor / 0.5f; // �̵��Ÿ� * scaleFactor / 0.5f ��ŭ ũ�� ��ȭ (y��)

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